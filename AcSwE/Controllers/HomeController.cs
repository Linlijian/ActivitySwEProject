using System.Linq;
using System.Web.Mvc;
using AcSwE.Models;

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
                var user = db.Teachers.Where(x => x.username == t.username).FirstOrDefault();
                var userid = user.id;
                if (user == null)
                {
                    ViewBag.user = "Wrong username or password";
                    return View(user);
                }
                else
                {
                    Session["user"] = user.firstName;
                    Session["id"] = user.id;
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