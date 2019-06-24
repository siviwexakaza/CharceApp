using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CharceApp.Models;

namespace CharceApp.Controllers
{
    public class ProfilePicFileController : Controller
    {
        //Returns profile picture of an individual user (Personal Account or Business Account)
        ApplicationDbContext db = new ApplicationDbContext();

        public ActionResult Index(int id)
        {
            var pic = db.Files.FirstOrDefault(x => x.ProfilePicID == id);
            return File(pic.Content, pic.ContentType);
        }

        public ActionResult Business_Pic(int id)
        {
           // var pic = db.File_Businesses.FirstOrDefault(x => x.ProfilePic_BusinessID == id);
            var pic = db.File_Businesses.ToList().Where(x => x.BusinessId == id).FirstOrDefault();
            return File(pic.Content, pic.ContentType);

        }

        public ActionResult Product_Pic(int id)
        {
            // var pic = db.File_Businesses.FirstOrDefault(x => x.ProfilePic_BusinessID == id);
            var pic = db.File_Products.ToList().Where(x => x.BusinessId == id).FirstOrDefault();
            return File(pic.Content, pic.ContentType);

        }

        public ActionResult ProductPic(int id)
        {
            // var pic = db.File_Businesses.FirstOrDefault(x => x.ProfilePic_BusinessID == id);
            var pic = db.File_Products.ToList().Where(x => x.ProfilePic_ProductID == id).FirstOrDefault();
            return File(pic.Content, pic.ContentType);

        }

        public ActionResult OpenPersonalPic(string app_user_id)
        {
            PersonalAccount p_acc = db.personalaccounts.ToList().Where(x => x.AppUserId == app_user_id).FirstOrDefault();
            ProfilePic ppic = db.profilepics.ToList().Where(x => x.UserId == p_acc.ID).FirstOrDefault();


            if (ppic != null)
            {
                var pic = db.Files.OrderByDescending(x => x.ProfilePicID == ppic.ID).FirstOrDefault();
                return File(pic.Content, pic.ContentType);
            }
            else
            {
                return File("/Content/default.jpg", "image/jpeg");
            }

        }
    }
}