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
using System.Data.SqlClient;
using System.Data.OleDb;
using System.Configuration;
using System.Text.RegularExpressions;

namespace AcSwE.Areas.Admin.Controllers
{
    public class ActivitieController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);
        OleDbConnection Econ;

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

        [HttpGet,ActionName("Index")]
        public ActionResult Search(string word)
        {
            //check int or str            
            if (word != null)
            {
                //convert str to int
                int number;
                Int32.TryParse(word, out number);
                //search
                bool allDigits = word.All(char.IsDigit);
                if (allDigits)
                {
                    return View(db.Activitys.Where(x => x.yearStd == number ||
                                word == null || x.yearStudy == number || x.countStd == number).ToList());
                }
                else
                {
                    return View(db.Activitys.Where(x => x.activityname.Contains(word) ||
                                word == null || x.detail.Contains(word) || x.endDate.Contains(word)
                                || x.startDate.Contains(word) || x.location.Contains(word) ||
                                x.locationPoint.Contains(word) || x.room.Contains(word)).ToList());
                    //return View(db.Activitys.Where(x => x.activityname.StartsWith(word) ||
                    //            word == null || x.detail.StartsWith(word) || x.endDate.StartsWith(word)
                    //            || x.startDate.StartsWith(word) || x.location.StartsWith(word) ||
                    //            x.locationPoint.StartsWith(word) || x.room.StartsWith(word)).ToList());
                }

            }
            return View(db.Activitys.ToList());
        }

        // GET: Admin/Activitie/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                string baseUrl1 = Request.Url.Scheme + "://" + Request.Url.Authority +
                Request.ApplicationPath.TrimEnd('/') + "/" + "Error/BadRequest/";
                return Redirect(baseUrl1);
            }
            ViewBag.TEA = (from s in db.Teachers
                           join d in db.Joins on s.id equals d.idTea
                           where d.idActivity == id
                           select s).ToList();
            Activity activity = db.Activitys.Find(id);
            string replaceWith = " ";
            string detail = activity.detail.ToString();
            ViewBag.result = Regex.Replace(detail, @"\r\n?|\n", replaceWith);
            var data = (from a in db.Joins where a.idActivity == id select a).ToList();
            StudentTemp t = new StudentTemp();
            for (int i = 0; i < data.Count(); i++)
            {
                t.idStd = data[i].idStd;
                db.StudentTemps.Add(t);
                db.SaveChanges();
            }

            ViewBag.std = (from q in db.StudentTemps join w in db.Students on q.idStd equals w.idStd select w).ToList();

            ViewBag.acid = id;

            var temp = db.StudentTemps.ToList();
            StudentTemp std = new StudentTemp();
            for (int i = 0; i < temp.Count(); i++)
            {
                std = db.StudentTemps.Find(temp[i].id);
                db.StudentTemps.Remove(std);
                db.SaveChanges();
            }
            if (activity == null)
            {
                string baseUrl1 = Request.Url.Scheme + "://" + Request.Url.Authority +
                Request.ApplicationPath.TrimEnd('/') + "/" + "Error/PageNotFound/";
                return Redirect(baseUrl1);
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

        public void Add_Std_withImport(HttpPostedFileBase file, int id, int Edit)
        {
            string filename = Guid.NewGuid() + Path.GetExtension(file.FileName);
            string filepath = "/Content/excelfolder/" + filename;
            file.SaveAs(Path.Combine(Server.MapPath("/Content/excelfolder"), filename));
            InsertExceldata(filepath, filename);

            if (Edit != 0)
            {

                var data2 = (from a in db.Joins join ss in db.StudentTemps on a.idStd equals ss.idStd where a.idActivity == Edit select ss).ToList();
                //del equals
                for (int i = 0; i < data2.Count(); i++)
                {
                    StudentTemp t = db.StudentTemps.Find(data2[i].id);
                    db.StudentTemps.Remove(t);
                    db.SaveChanges();
                }
                //try
                //{
                //    var data2 = (from a in db.Joins join ss in db.StudentTemps on a.idStd equals ss.idStd where a.idActivity == Edit select ss).ToList();
                //    //del equals
                //    for (int i = 0; i < data2.Count(); i++)
                //    {
                //        StudentTemp t = db.StudentTemps.Find(data2[i].id);
                //        db.StudentTemps.Remove(t);
                //        db.SaveChanges();
                //    }
                //}
                //catch (Exception)
                //{
                //    var temp = db.StudentTemps.ToList();
                //    for (int i = 0; i < temp.Count(); i++)
                //    {
                //        StudentTemp tempStd = db.StudentTemps.Find(temp[i].id);
                //        db.StudentTemps.Remove(tempStd);
                //        db.SaveChanges();
                //    }
                //    return;
                //}
                
            }
            var data = (from a in db.Students
                        join b in db.StudentTemps
                        on a.idStd equals b.idStd
                        select b).ToList();
            Join j = new Join();
            j.idActivity = id;
            for (int i = 0; i < data.Count(); i++)
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
            for (int i = 0; i < st.Count(); i++)
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
        public ActionResult Create(HttpPostedFileBase xlnx, HttpPostedFileBase file, [Bind(Include = "id,activityname,location,teacherInActivity,yearStd,yearStudy,startDate,endDate,img,locationPoint,room,detail")] Activity activity)
        {
            if (ModelState.IsValid)
            {
                Join j = new Join();
                int Edit = 0;
                if (file == null)
                {
                    activity.img = "default.jpg";
                    j.idTea = activity.teacherInActivity;
                    db.Activitys.Add(activity);
                    db.SaveChanges();
                    Activity aa = db.Activitys.Find(activity.id);
                    j.idActivity = aa.id;
                    if (xlnx != null)
                        Add_Std_withImport(xlnx, aa.id, Edit);
                    db.Joins.Add(j);
                    aa.countStd = (from xx in db.Joins where xx.idActivity == activity.id select xx).Count();
                    db.Entry(aa).State = EntityState.Modified;
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
                if (xlnx != null)
                    Add_Std_withImport(xlnx, a.id, Edit);
                db.Joins.Add(j);
                a.countStd = (from xx in db.Joins where xx.idActivity == activity.id select xx).Count();
                db.Entry(a).State = EntityState.Modified;
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
                string baseUrl1 = Request.Url.Scheme + "://" + Request.Url.Authority +
                Request.ApplicationPath.TrimEnd('/') + "/" + "Error/BadRequest/";
                return Redirect(baseUrl1);
            }
            Activity activity = db.Activitys.Find(id);
            activity.TeacherList = db.Teachers.ToList<Teacher>();
            if (activity == null)
            {
                string baseUrl1 = Request.Url.Scheme + "://" + Request.Url.Authority +
                Request.ApplicationPath.TrimEnd('/') + "/" + "Error/PageNotFound/";
                return Redirect(baseUrl1);


            }
            return View(activity);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(HttpPostedFileBase xlnx, HttpPostedFileBase file, Activity activity)
        {
            if (ModelState.IsValid)
            {
                //id,activityname,location,teacherInActivity,yearStd,yearStudy,startDate,endDate,img,locationPoint,room,detail
                var q = (from w in db.Joins where w.idActivity == activity.id select w).ToList();
                Join j = new Join();
                int id = activity.id;
                int Edit = activity.id;
                if (file == null)
                {
                    //activity.img = "default.jpg";  
                    Activity ac = db.Activitys.Find(activity.id);
                    ac.activityname = activity.activityname;
                    ac.location = activity.location;
                    ac.teacherInActivity = activity.teacherInActivity;
                    ac.yearStd = activity.yearStd;
                    ac.yearStudy = activity.yearStudy;
                    ac.startDate = activity.startDate;
                    ac.endDate = activity.endDate;
                    ac.locationPoint = activity.locationPoint;
                    ac.room = activity.room;
                    ac.detail = activity.detail;

                    j = db.Joins.Find(q[q.Count() - 1].id);
                    j.idTea = activity.teacherInActivity;
                    db.Entry(ac).State = EntityState.Modified;
                    db.SaveChanges();
                    Activity aa = db.Activitys.Find(activity.id);
                    if (xlnx != null)
                        Add_Std_withImport(xlnx, id, Edit);
                    aa.countStd = (from xx in db.Joins where xx.idActivity == activity.id select xx).Count();
                    aa.countStd -= 1;
                    db.Entry(aa).State = EntityState.Modified;
                    db.Entry(j).State = EntityState.Modified;
                    db.SaveChanges();

                    return RedirectToAction("EditStd", new { id });
                }

                file.SaveAs(HttpContext.Server.MapPath("~/Content/img/activity/")
                              + file.FileName);
                activity.img = file.FileName;
                j = db.Joins.Find(q[q.Count() - 1].id);
                j.idTea = activity.teacherInActivity;
                db.Entry(activity).State = EntityState.Modified;
                db.SaveChanges();
                Activity a = db.Activitys.Find(activity.id);
                if (xlnx != null)
                    Add_Std_withImport(xlnx, id, Edit);
                db.Entry(j).State = EntityState.Modified;
                a.countStd = (from xx in db.Joins where xx.idActivity == activity.id select xx).Count();
                a.countStd -= 1;
                db.Entry(a).State = EntityState.Modified;
                db.SaveChanges();

                return RedirectToAction("EditStd", new { id });
            }
            return View(activity);
        }

        public ActionResult EditStd(int? id)
        {
            if (id == null)
            {
                string baseUrl1 = Request.Url.Scheme + "://" + Request.Url.Authority +
                Request.ApplicationPath.TrimEnd('/') + "/" + "Error/BadRequest/";
                return Redirect(baseUrl1);
            }
            var data = (from a in db.Joins where a.idActivity == id select a).ToList();
            StudentTemp t = new StudentTemp();
            for (int i = 0; i < data.Count(); i++)
            {
                t.idStd = data[i].idStd;
                db.StudentTemps.Add(t);
                db.SaveChanges();
            }
            ViewBag.std = (from q in db.StudentTemps join w in db.Students on q.idStd equals w.idStd select w).ToList();
            //var std = (from q in db.StudentTemps join w in db.Students on q.idStd equals w.idStd select w).ToList();
            ViewBag.acid = id;
            var temp = db.StudentTemps.ToList();
            StudentTemp std = new StudentTemp();
            for (int i = 0; i < temp.Count(); i++)
            {
                std = db.StudentTemps.Find(temp[i].id);
                db.StudentTemps.Remove(std);
                db.SaveChanges();
            }
            return View();

        }

        public ActionResult EditStds(int? id, int Acid)
        {
            // string baseUrl = Request.Url.GetLeftPart(UriPartial.Authority);
            string baseUrl = Request.Url.Scheme + "://" + Request.Url.Authority +
     Request.ApplicationPath.TrimEnd('/') + "/" + "Admin/Activitie/EditStd/" + Acid;
            return RedirectToAction("Edit", "Student", new { id, baseUrl });
        }
        public ActionResult DetailsStd(int? id, int Acid)
        {
            string baseUrl = Request.Url.Scheme + "://" + Request.Url.Authority +
     Request.ApplicationPath.TrimEnd('/') + "/" + "Admin/Activitie/EditStd/" + Acid;
            return RedirectToAction("Details", "Student", new { id, baseUrl });
        }
        public ActionResult DeleteStd(int? id, int Acid,int idstd)
        {
            string baseUrl = Request.Url.Scheme + "://" + Request.Url.Authority +
     Request.ApplicationPath.TrimEnd('/') + "/" + "Admin/Activitie/EditStd/" + Acid;
            return RedirectToAction("del", "Student", new { id, baseUrl,Acid , idstd });
        }

        // GET: Admin/Activitie/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                string baseUrl1 = Request.Url.Scheme + "://" + Request.Url.Authority +
                Request.ApplicationPath.TrimEnd('/') + "/" + "Error/PageNotFound/";
                return Redirect(baseUrl1);
            }
            Activity activity = db.Activitys.Find(id);
            if (activity == null)
            {
                string baseUrl1 = Request.Url.Scheme + "://" + Request.Url.Authority +
                Request.ApplicationPath.TrimEnd('/') + "/" + "Error/PageNotFound/";
                return Redirect(baseUrl1);
            }
            return View(activity);
        }

        // POST: Admin/Activitie/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {      
            Activity activity = db.Activitys.Find(id);
            var j = (from a in db.Joins where a.idActivity == id select a).ToList();
            for (int i = 0; i < j.Count(); i++)
            {
                db.Joins.Remove(j[i]);
                db.SaveChanges();
            }
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