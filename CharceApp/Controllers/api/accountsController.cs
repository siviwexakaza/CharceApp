using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CharceApp.Models;

namespace CharceApp.Controllers.api
{
    public class accountsController : Controller
    {
        ApplicationDbContext db = new ApplicationDbContext();
        // GET: accounts
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public JsonResult users()
        {
            List<ApplicationUser> users = db.Users.ToList();
            return Json(users, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult personal_accounts()
        {
            List<PersonalAccount> accounts = db.personalaccounts.ToList();
            return Json(accounts, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult business_accounts()
        {
            List<BusinessAccount> accounts = db.businessaccounts.ToList();
            return Json(accounts, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult products()
        {
            List<ProfilePic_Product> products = db.profilepic_products.ToList();
            return Json(products, JsonRequestBehavior.AllowGet);
        }

        

    }
}