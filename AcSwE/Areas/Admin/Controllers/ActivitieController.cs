﻿using System;
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
using System.Data.SqlClient;
using System.Data.OleDb;
using System.Configuration;

namespace AcSwE.Areas.Admin.Controllers
{
    public class ActivitieController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);
        OleDbConnection Econ;
        int idAC;
        private void ExcelConn(string filepath)
        {
            string constr = string.Format(@"Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0};Extended Properties=""Excel 12.0 Xml;HDR=YES;""", filepath);

            Econ = new OleDbConnection(constr);
        }

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

        public void Add_Std_withImport(HttpPostedFileBase file,int id)
        {
            string filename = Guid.NewGuid() + Path.GetExtension(file.FileName);
            string filepath = "/Content/excelfolder/" + filename;
            file.SaveAs(Path.Combine(Server.MapPath("/Content/excelfolder"), filename));
            InsertExceldata(filepath, filename);

            var data = (from a in db.Students
                        join b in db.StudentTemps
                        on a.idStd equals b.idStd
                        select b).ToList();
            Join j = new Join();
            j.idActivity = id;
            for(int i = 0; i < data.Count(); i++)
            {
                j.idStd = data[i].idStd;                
                db.Joins.Add(j);
                int a = data[i].id;
                StudentTemp t = db.StudentTemps.Find(a);
                db.StudentTemps.Remove(t);
                db.SaveChanges();
            }

            var st = db.StudentTemps.ToList();
            Student s = new Student();
            for(int i = 0; i < st.Count(); i++)
            {
                s.idStd = st[i].idStd;
                s.firstName = st[i].firstName;
                s.lastName = st[i].lastName;
                s.title = st[i].title;
                s.img = st[i].img;
                s.year = st[i].year;
                j.idStd = st[i].idStd;
                db.Joins.Add(j);
                db.Students.Add(s);
                db.SaveChanges();
            }

            for (int i = 0; i < st.Count(); i++)
            {
                StudentTemp t = db.StudentTemps.Find(st[i].id);
                db.StudentTemps.Remove(t);
                db.SaveChanges();
            }

           
        }

        private void InsertExceldata(string fileepath, string filename)
        {
            string fullpath = Server.MapPath("/Content/excelfolder/") + filename;
            ExcelConn(fullpath);
            string query = string.Format("Select * from [{0}]", "Sheet1$");
            OleDbCommand Ecom = new OleDbCommand(query, Econ);
            Econ.Open();

            DataSet ds = new DataSet();
            OleDbDataAdapter oda = new OleDbDataAdapter(query, Econ);
            Econ.Close();
            oda.Fill(ds);

            DataTable dt = ds.Tables[0];
            SqlBulkCopy objbulk = new SqlBulkCopy(con);
            objbulk.DestinationTableName = "StudentTemp";
            objbulk.ColumnMappings.Add("title", "title");
            objbulk.ColumnMappings.Add("idStd", "idStd");
            objbulk.ColumnMappings.Add("firstName", "firstName");
            objbulk.ColumnMappings.Add("lastName", "lastName");
            objbulk.ColumnMappings.Add("year", "year");

            con.Open();
            objbulk.WriteToServer(dt);
            con.Close();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(HttpPostedFileBase xlnx, HttpPostedFileBase file, [Bind(Include = "id,activityname,location,teacherInActivity,yearStd,yearStudy,startDate,endDate,img,locationPoint,room")] Activity activity)
        {
            if (ModelState.IsValid)
            {
                Join j = new Join();
                if (file == null)
                {                   
                    activity.img = "default.jpg";
                    j.idTea = activity.teacherInActivity;
                    db.Activitys.Add(activity);
                    db.SaveChanges();
                    Activity aa = db.Activitys.Find(activity.id);
                    j.idActivity = aa.id;
                    Add_Std_withImport(xlnx, aa.id);
                    db.Joins.Add(j);
                    db.SaveChanges();
                    return RedirectToAction("index");
                }
                
                file.SaveAs(HttpContext.Server.MapPath("~/Content/img/activity/")
                              + file.FileName);
                activity.img = file.FileName;
                j.idTea = activity.teacherInActivity;
                db.Activitys.Add(activity);
                db.SaveChanges();
                Activity a = db.Activitys.Find(activity.id);
                j.idActivity = a.id;
                Add_Std_withImport(xlnx, a.id);
                db.Joins.Add(j);
                db.SaveChanges();
                return RedirectToAction("index");
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
            activity.TeacherList = db.Teachers.ToList<Teacher>();
            if (activity == null)
            {
                return HttpNotFound();
            }
            return View(activity);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(HttpPostedFileBase file, [Bind(Include = "id,activityname,location,teacherInActivity,yearStd,yearStudy,startDate,endDate,img,locationPoint,room")] Activity activity)
        {
            if (ModelState.IsValid)
            {
                Join j = db.Joins.Find(activity.id);

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

                    j.idTea = activity.teacherInActivity;
                    aaa.img = "default.jpg";
                    db.Entry(aaa).State = EntityState.Modified;
                    db.SaveChanges();
                   // EditStd(aaa.id);
                    return RedirectToAction("EditStd",new { aaa.id });
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
                j.idTea = activity.teacherInActivity;

                aa.img = file.FileName;
                db.Entry(aa).State = EntityState.Modified;

                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(activity);
        }

        public ActionResult EditStd(int ?id)
        {
            var data = (from a in db.Joins where a.idActivity == id select a).ToList();
            StudentTemp t = new StudentTemp();
            for(int i = 0; i < data.Count(); i++)
            {
                t.idStd = data[i].idStd;
                db.StudentTemps.Add(t);
                db.SaveChanges();
            }
             ViewBag.std = (from q in db.StudentTemps join w in db.Students on q.idStd equals w.idStd select w).ToList();
             //var std = (from q in db.StudentTemps join w in db.Students on q.idStd equals w.idStd select w).ToList();

            int j = 0;
            return View();
           
        }


        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult EditStd()
        //{
  
        //    return View();

        //}

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
