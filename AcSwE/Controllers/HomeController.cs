﻿using System.Linq;
using System.Web.Mvc;
using AcSwE.Models;
using System;

namespace AcSwE.Controllers
{
    public class HomeController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        // GET: Home
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(Teacher t)
        {
            using (db)
            {
                //var user = db.Teachers.Where(x => x.username == t.username && x.password == t.password).FirstOrDefault();
                var user = db.Teachers.Where(x => x.username == t.username && x.password == t.password).FirstOrDefault();
               
                if (user == null)
                {
                    ViewBag.user = "Wrong username or password";
                    return View(user);
                }
                else
                {
                    var userid = user.id;
                    Session["username"] = user.firstName;                    
                    Session["user"] = user.status;
                    if(user.status == "Admin     ")
                    {
                        Session["status"] = user.status;
                    }
                    else
                    {
                        Session["status"] = null;
                    }
                    Session["id"] = user.id;
                    Session["uel"] = Request.Url.GetLeftPart(UriPartial.Authority);
                    return RedirectToAction("Index");
                }
            }
                
        }

        public ActionResult Logout()
        {
            Session.Abandon();
            return RedirectToAction("Index");
        }
       
            public ActionResult Details()
        {
            
            return RedirectToAction("Details","Teacher");
        }
    }
}