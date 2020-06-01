using Login.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Login.Controllers
{
    public class LoginController : Controller
    {
        //
        // GET: /Login/
        WeDBEntities1 db = new WeDBEntities1();
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Index(string name, string pas)
        {
            var b = (from m in db.User where m.name == name && m.password == pas select m).FirstOrDefault();
            if (b == null)
            {
                return Content("无账号");
            }
            return Redirect("home/index");
        }

    }
}
