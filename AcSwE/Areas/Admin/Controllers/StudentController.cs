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
            ViewBag.count = (from a in db.Joins where student.idStd == a.idStd select a).Count();
            var data = (from s in db.Joins where s.idStd == student.idStd select s).ToList();
            StudentTemp t = new StudentTemp();
            for (int i = 0; i < data.Count(); i++)
            {
                t.idStd = data[i].idActivity;
                db.StudentTemps.Add(t);
                db.SaveChanges();
            }
            ViewBag.ST_AC = (from z in db.StudentTemps join x in db.Activitys on z.idStd equals x.id select x).ToList();
          
            //del
            var temp = db.StudentTemps.ToList();
            StudentTemp std = new StudentTemp();
            for (int i = 0; i < temp.Count(); i++)
            {
                std = db.StudentTemps.Find(temp[i].id);
                db.StudentTemps.Remove(std);
                db.SaveChanges();
            }

            if (student == null)
            {
                return HttpNotFound();
            }
            ViewBag.url = baseUrl;
            //if (baseUrl != null)
            //    return Redirect(baseUrl);           
            return View(student);
        }

        public ActionResult CallDetails(int? id)
        {
            return RedirectToAction("Details", "Activitie", new { id });
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

        public ActionResult del(int id,string baseUrl,int Acid, int idstd)
        {
            //Student student = db.Students.Find(id);
            var j = (from d in db.Joins where Acid == d.idActivity select d).ToList();
            for (int i = 0; i < j.Count(); i++)
            {
                if (idstd == j[i].idStd)
                {
                    Join jj = db.Joins.Find(j[i].id);
                    db.Joins.Remove(jj);
                    break;
                }
            }
           // db.Students.Remove(student);            
            db.SaveChanges();
            Activity a = db.Activitys.Find(Acid);            
            a.countStd = (from xx in db.Joins where xx.idActivity == Acid select xx).Count();
            a.countStd -= 1;
            db.Entry(a).State = EntityState.Modified;
            db.SaveChanges();
            return Redirect(baseUrl);
        }
    }
}
