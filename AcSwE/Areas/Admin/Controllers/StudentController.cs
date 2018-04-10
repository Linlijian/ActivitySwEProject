using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using AcSwE.Models;

namespace AcSwE.Areas.Admin.Controllers
{
    public class StudentController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Admin/Student
        public ActionResult Index()
        {
            return View(db.Students.ToList());
        }

        // GET: Admin/Student/Details/5
        public ActionResult Details(string baseUrl, int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Student student = db.Students.Find(id);
            if (student == null)
            {
                return HttpNotFound();
            }
            ViewBag.url = baseUrl;
            //if (baseUrl != null)
            //    return Redirect(baseUrl);           
            return View(student);
        }
        
        [HttpPost]
        public ActionResult callBack(string baseUrl)
        {
            if(baseUrl != null)
                 return Redirect(baseUrl);
            return RedirectToAction("Index");
        }

        // GET: Admin/Student/Create
        public ActionResult Create()
        {
            return View();
        }
              
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(HttpPostedFileBase file,[Bind(Include = "id,title,idStd,firstName,lastName,year,img")] Student student)
        {
            if (ModelState.IsValid)
            {
                if(file == null)
                {
                    student.img = "default.jpg";
                    db.Students.Add(student);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                file.SaveAs(HttpContext.Server.MapPath("~/Content/img/student/")
                                 + file.FileName);
                student.img = file.FileName;
                db.Students.Add(student);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(student);
        }

        // GET: Admin/Student/Edit/5
        public ActionResult Edit(int? id,string baseUrl)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Student student = db.Students.Find(id);
            if (student == null)
            {
                return HttpNotFound();
            }
            ViewBag.baseUrl = baseUrl;
            return View(student);
        }
               
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(HttpPostedFileBase file,string baseUrl,Student student)
        {
            if (ModelState.IsValid)
            {
                if(file == null)
                {
                    Student s = db.Students.Find(student.id);
                    
                    s.id = student.id;
                    s.firstName = student.firstName;
                    s.lastName = student.lastName;
                    s.year = student.year;
                    //s.img = student.img;
                    s.title = student.title;
                             
                    db.Entry(s).State = EntityState.Modified;
                    db.SaveChanges();
                    if (baseUrl != null)
                        return Redirect(baseUrl);
                    return RedirectToAction("Index");
                }
                file.SaveAs(HttpContext.Server.MapPath("~/Content/img/student/")
                                + file.FileName);
                student.img = file.FileName;
                db.Entry(student).State = EntityState.Modified;
                db.SaveChanges();
                if(baseUrl != null)
                    return Redirect(baseUrl);
                return RedirectToAction("Index");
            }
            return View(student);
        }

        // GET: Admin/Student/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Student student = db.Students.Find(id);
            if (student == null)
            {
                return HttpNotFound();
            }
            return View(student);
        }

        // POST: Admin/Student/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Student student = db.Students.Find(id);
            db.Students.Remove(student);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        public ActionResult del(int id,string baseUrl)
        {
            Student student = db.Students.Find(id);
            db.Students.Remove(student);
            db.SaveChanges();
            return Redirect(baseUrl);
        }
    }
}
