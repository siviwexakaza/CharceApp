using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CharceApp.Models;
using Microsoft.AspNet.Identity;

namespace CharceApp.Controllers
{
    public class profilesController : Controller
    {
        ApplicationDbContext db = new ApplicationDbContext();
       /**

        Creates a new business account for a user
        E.g User X creates an account for an IT Company
       
        **/

        public JsonResult AddProfile(BusinessAccountVM obj)
        {
            string myId = User.Identity.GetUserId();
            PersonalAccount pa = db.personalaccounts.ToList().Where(x => x.AppUserId == myId).FirstOrDefault();

            BusinessAccount ba = new BusinessAccount() {
                BusinessName=obj.BusinessName, BusinessType=obj.BusinessType,
                Email=obj.Email,Phone=obj.Phone,Website=obj.Website,Location=
                obj.Location,PersonalAccountID=pa.ID
            };

            db.businessaccounts.Add(ba);
            db.SaveChanges();

            return Json("Account Added", JsonRequestBehavior.AllowGet);
        }


    }
}