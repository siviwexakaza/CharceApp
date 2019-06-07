using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CharceApp.Controllers
{
    public class pagesController : Controller
    {
        // GET: pages
        public ActionResult Index()
        {
            return View();
        }
    }
}