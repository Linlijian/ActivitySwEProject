using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using AcSwE.Models;
using System.Drawing;
using System.IO;
using AcSwE.Extensions;

namespace AcSwE.Areas.Admin.Controllers
{
    public class ActivitieController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Admin/Activitie
        public ActionResult Index()
        {
            return View(db.Activitys.ToList());
        }

        // GET: Admin/Activitie/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Activity activity = db.Activitys.Find(id);
            if (activity == null)
            {
                return HttpNotFound();
            }
            return View(activity);
        }

        // GET: Admin/Activitie/Create
        public ActionResult Create()
        {
            Activity a = new Activity();
            using (db)
            {
                a.TeacherList = db.Teachers.ToList<Teacher>();
            }
            return View(a);
            
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(HttpPostedFileBase file,[Bind(Include = "id,activityname,location,teacherInActivity,yearStd,yearStudy,startDate,endDate,img,locationPoint,room")] Activity activity)
        {
            if (ModelState.IsValid)
            {
                if(file == null)
                {                    
                    activity.img = "default.jpg";
                    db.Activitys.Add(activity);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }

                file.SaveAs(HttpContext.Server.MapPath("~/Content/img/activity/")
                              + file.FileName);
                activity.img = file.FileName;
                db.Activitys.Add(activity);
                db.SaveChanges();
                return RedirectToAction("Index");



            }
            
            return View(activity);
        }      
               
        // GET: Admin/Activitie/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Activity activity = db.Activitys.Find(id);
            if (activity == null)
            {
                return HttpNotFound();
            }
            return View(activity);
        }
                
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(HttpPostedFileBase file,[Bind(Include = "id,activityname,location,teacherInActivity,yearStd,yearStudy,startDate,endDate,img,locationPoint,room")] Activity activity)
        {
            if (ModelState.IsValid)
            {

               
                    if (file == null)
                {
                    Activity aaa = db.Activitys.Find(activity.id);
                    aaa.id = activity.id;
                    aaa.activityname = activity.activityname.ToString();
                    aaa.endDate = activity.endDate.ToString();
                    aaa.startDate = activity.startDate.ToString();
                    aaa.location = activity.location.ToString();
                    aaa.teacherInActivity = activity.teacherInActivity;
                    aaa.room = activity.room.ToString();
                    aaa.locationPoint = activity.locationPoint.ToString();
                    aaa.yearStd = activity.yearStd;
                    aaa.yearStudy = activity.yearStudy;
                   
                    aaa.img = "default.jpg";
                    db.Entry(aaa).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }

                file.SaveAs(HttpContext.Server.MapPath("~/Content/img/activity/")
                              + file.FileName);
                activity.img = file.FileName;

                Activity aa = db.Activitys.Find(activity.id);
                aa.id = activity.id;
                aa.activityname = activity.activityname.ToString();
                aa.endDate = activity.endDate.ToString();
                aa.startDate = activity.startDate.ToString();
                aa.location = activity.location.ToString();
                aa.teacherInActivity = activity.teacherInActivity;
                aa.room = activity.room.ToString();
                aa.locationPoint = activity.locationPoint.ToString();
                aa.yearStd = activity.yearStd;
                aa.yearStudy = activity.yearStudy;

                aa.img = file.FileName;
                db.Entry(aa).State = EntityState.Modified;

                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(activity);
        }

        // GET: Admin/Activitie/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Activity activity = db.Activitys.Find(id);
            if (activity == null)
            {
                return HttpNotFound();
            }
            return View(activity);
        }

        // POST: Admin/Activitie/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Activity activity = db.Activitys.Find(id);
            db.Activitys.Remove(activity);
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
                
    }
}
