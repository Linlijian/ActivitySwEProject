using AcSwE.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace AcSwE.Controllers
{
    public class ActivityController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        // GET: Activity
        public ActionResult Index()
        {
            return View(db.Activitys.ToList());
        }
        [HttpGet, ActionName("Index")]
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
        // GET: Activity/Details/5
        public ActionResult Details(int ?id)
        {
            if (id == null)
            {
                string baseUrl = Request.Url.Scheme + "://" + Request.Url.Authority +
                Request.ApplicationPath.TrimEnd('/') + "/" + "Error/BadRequest/";
                return Redirect(baseUrl);
            }
            ViewBag.TEA = (from s in db.Teachers
                           join d in db.Joins on s.id equals d.idTea
                           where d.idActivity == id
                           select s).ToList();
            Activity activity = db.Activitys.Find(id);
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
                return HttpNotFound();
            }
            return View(activity);
        }
        public ActionResult DetailsStd(int? id, int Acid)
        {
            string baseUrl = Request.Url.Scheme + "://" + Request.Url.Authority +
            Request.ApplicationPath.TrimEnd('/') + "/" + "Activity/Details/" + Acid;
            return RedirectToAction("Details", "Studented", new { id, baseUrl });
        }



    }
}
