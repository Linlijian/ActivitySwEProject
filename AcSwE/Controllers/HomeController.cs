using System.Linq;
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
            ViewBag.New = (from a in db.Activitys orderby a.id descending select a).ToList();
            ViewBag.TEA = (from s in db.Teachers join d in db.Activitys on s.id equals d.teacherInActivity
                           orderby d.id descending select s).ToList();
            ViewBag.COUNT = (from a in db.Activitys orderby a.countStd descending select a).ToList();
            int ii = 0;
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
                    if(user.status == "Admin")
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