﻿using AcSwE.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace AcSwE.Controllers
{
    public class StudentedController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Admin/Student
        public ActionResult Index()
        {
            return View(db.Students.ToList());
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
                    return View(db.Students.Where(x => x.idStd == number).ToList());
                }
                else
                {
                    return View(db.Students.Where(x => x.year.Contains(word) ||
                                word == null || x.firstName.Contains(word) || x.lastName.Contains(word)
                                || x.title.Contains(word)).ToList());
                    //return View(db.Activitys.Where(x => x.activityname.StartsWith(word) ||
                    //            word == null || x.detail.StartsWith(word) || x.endDate.StartsWith(word)
                    //            || x.startDate.StartsWith(word) || x.location.StartsWith(word) ||
                    //            x.locationPoint.StartsWith(word) || x.room.StartsWith(word)).ToList());
                }

            }
            return View(db.Students.ToList());
        }

        public ActionResult Details(string baseUrl, int? id)
        {
            if (id == null)
            {
                string baseUrl1 = Request.Url.Scheme + "://" + Request.Url.Authority +
                Request.ApplicationPath.TrimEnd('/') + "/" + "Error/BadRequest/";
                return Redirect(baseUrl1);
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
                string baseUrl1 = Request.Url.Scheme + "://" + Request.Url.Authority +
                Request.ApplicationPath.TrimEnd('/') + "/" + "Error/PageNotFound/";
                return Redirect(baseUrl1);
            }
            ViewBag.url = baseUrl;
            //if (baseUrl != null)
            //    return Redirect(baseUrl);           
            return View(student);
        }

        public ActionResult CallDetails(int? id)
        {
            return RedirectToAction("Details", "Activity", new { id });
        }

        [HttpPost]
        public ActionResult callBack(string baseUrl)
        {
            if (baseUrl != null)
                return Redirect(baseUrl);
            return RedirectToAction("Index");
        }

    }
}
