﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using AcSwE.Models;
using AcSwE.Extensions;

namespace AcSwE.Areas.Admin.Controllers
{
    public class TeacherController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Admin/Teacher
        public ActionResult Index()
        {
            var a = Session["status"];
            var aa = Session["uel"];
            string url = (string)(aa);
            if (a == null)
            {
                return Redirect(url);
            }
            return View(db.Teachers.ToList());
        }

        // GET: Admin/Teacher/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Teacher teacher = db.Teachers.Find(id);
            if (teacher == null)
            {
                return HttpNotFound();
            }
            return View(teacher);
        }

        // GET: Admin/Teacher/Create
        public ActionResult Create()
        {
            return View();
        }      
    
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(HttpPostedFileBase file,Teacher teacher)
        {
            if (ModelState.IsValid)
            {
                if (file == null)
                {
                    teacher.img = "default.jpg";
                    db.Teachers.Add(teacher);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }

                file.SaveAs(HttpContext.Server.MapPath("~/Content/img/teacher/")
                                  + file.FileName);
                teacher.img = file.FileName;
                db.Teachers.Add(teacher);
                db.SaveChanges();
                
            }
            return RedirectToAction("Index", "Teacher");
            //return View(teacher);
        }

        // GET: Admin/Teacher/Edit/5
        public ActionResult Edit(int? id)
        {            
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Teacher teacher = db.Teachers.Find(id);
            if (teacher == null)
            {
                return HttpNotFound();
            }
            return View(teacher);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(HttpPostedFileBase file,[Bind(Include = "id,title,firstName,lastName,username,password,status,img")] Teacher teacher)
        {
            if (ModelState.IsValid)
            {
                if(file == null)
                {
                    //teacher.img = "default.jpg";
                    db.Entry(teacher).State = EntityState.Modified;
                    db.SaveChanges();
                    Session["username"] = teacher.firstName;
                    return RedirectToAction("Index");
                }
                file.SaveAs(HttpContext.Server.MapPath("~/Content/img/teacher/")
                                 + file.FileName);
                teacher.img = file.FileName;
                db.Entry(teacher).State = EntityState.Modified;
                db.SaveChanges();
                Session["username"] = teacher.firstName;
                return RedirectToAction("Index");
            }
            return View(teacher);
        }

        // GET: Admin/Teacher/Delete/5
        public ActionResult Delete(int? id)
        {
            var a = Session["status"];
            var aa = Session["uel"];
            string url = (string)(aa);
            if (a == null)
            {
                return Redirect(url);
            }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Teacher teacher = db.Teachers.Find(id);
            if (teacher == null)
            {
                return HttpNotFound();
            }
            return View(teacher);
        }

        // POST: Admin/Teacher/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Teacher teacher = db.Teachers.Find(id);
            db.Teachers.Remove(teacher);
            db.SaveChanges();
            return RedirectToAction("Index", "Teacher");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
