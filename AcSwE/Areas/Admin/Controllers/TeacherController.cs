using System;
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
                string baseUrl1 = Request.Url.Scheme + "://" + Request.Url.Authority +
                Request.ApplicationPath.TrimEnd('/') + "/" + "Error/BadRequest/";
                return Redirect(baseUrl1);
            }
            Teacher teacher = db.Teachers.Find(id); // id 2           
            var q = (from a in db.Joins where a.idTea == id select a).ToList();
            // id = 1107	idstd = 0	idac = 1060	idtea = 2
            StudentTemp t = new StudentTemp();
            for (int i = 0; i < q.Count(); i++)
            {
                t.idStd = q[i].idActivity;
                db.StudentTemps.Add(t);
                db.SaveChanges();
            }

            ViewBag.T_AC = (from z in db.StudentTemps join x in db.Activitys on z.idStd equals x.id select x).ToList();
            ViewBag.count = (from d in db.Joins where teacher.id == d.idTea select d).Count();
            //del
            var temp = db.StudentTemps.ToList();
            StudentTemp std = new StudentTemp();
            for (int i = 0; i < temp.Count(); i++)
            {
                std = db.StudentTemps.Find(temp[i].id);
                db.StudentTemps.Remove(std);
                db.SaveChanges();
            }

            if (teacher == null)
            {
                string baseUrl1 = Request.Url.Scheme + "://" + Request.Url.Authority +
                Request.ApplicationPath.TrimEnd('/') + "/" + "Error/PageNotFound/";
                return Redirect(baseUrl1);
            }
            return View(teacher);
        }

        public ActionResult CallDetails(int? id)
        {
            return RedirectToAction("Details", "Activitie", new { id });
        }

        // GET: Admin/Teacher/Create
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(HttpPostedFileBase file, Teacher teacher)
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
                string baseUrl1 = Request.Url.Scheme + "://" + Request.Url.Authority +
                Request.ApplicationPath.TrimEnd('/') + "/" + "Error/BadRequest/";
                return Redirect(baseUrl1);
            }
            Teacher teacher = db.Teachers.Find(id);
            if (teacher == null)
            {
                string baseUrl1 = Request.Url.Scheme + "://" + Request.Url.Authority +
                 Request.ApplicationPath.TrimEnd('/') + "/" + "Error/PageNotFound/";
                return Redirect(baseUrl1);
            }
            return View(teacher);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(HttpPostedFileBase file, Teacher teacher)
        {
            if (ModelState.IsValid)
            {
                if (file == null)
                {
                    Teacher a = db.Teachers.Find(teacher.id);

                    a.id = teacher.id;
                    a.firstName = teacher.firstName;
                    a.lastName = teacher.lastName;
                    a.username = teacher.username;
                    a.password = teacher.password;
                    a.status = teacher.status;
                    a.title = teacher.title;

                    db.Entry(a).State = EntityState.Modified;
                    db.SaveChanges();
                    Session["username"] = a.firstName;
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
                string baseUrl1 = Request.Url.Scheme + "://" + Request.Url.Authority +
                Request.ApplicationPath.TrimEnd('/') + "/" + "Error/BadRequest/";
                return Redirect(baseUrl1); ;
            }
            Teacher teacher = db.Teachers.Find(id);
            if (teacher == null)
            {
                string baseUrl1 = Request.Url.Scheme + "://" + Request.Url.Authority +
                Request.ApplicationPath.TrimEnd('/') + "/" + "Error/PageNotFound/";
                return Redirect(baseUrl1);
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