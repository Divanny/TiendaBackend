using Data.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebAPI.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            Mailing mailing = new Mailing();
            mailing.SendEmail("divannyjpm@gmail.com", "probando", "<div><h2>Probando</h2></div>");
            ViewBag.Title = "Home Page";

            return View();
        }
    }
}
