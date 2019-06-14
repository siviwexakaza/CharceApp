using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CharceApp.Models;

namespace CharceApp.Controllers
{
    public class ProfilePicsProductController : Controller
    {
        ApplicationDbContext db = new ApplicationDbContext();
        public ActionResult Index()
        {
            return View(db.profilepic_businesses.ToList().OrderByDescending(p => p.ID));
        }


        public ActionResult Create(int BusinessID)
        {
            ViewBag.BusinessID = BusinessID;
            return View();
        }

        // POST: ProfilePics/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,BusinessId")] ProfilePic_Product profilePic, HttpPostedFileBase pic, int BusinessID)
        {
            // string myId = User.Identity.GetUserId();
            // PersonalAccount p_acc = db.personalaccounts.ToList().Where(x => x.AppUserId == myId).FirstOrDefault();

            profilePic.BusinessId = BusinessID;


            if (ModelState.IsValid)
            {
                if (pic != null && pic.ContentLength > 0)
                {

                    var avatar = new File_Product
                    {

                        FileName = System.IO.Path.GetFileName(pic.FileName),
                        FileType_Product = FileType_Product.Avatar,
                        ContentType = pic.ContentType,
                        BusinessId = BusinessID
                    };

                    using (var reader = new System.IO.BinaryReader(pic.InputStream))
                    {
                        avatar.Content = reader.ReadBytes(pic.ContentLength);
                    }
                    profilePic.Files_Product = new List<File_Product> { avatar };
                }
                
                db.profilepic_products.Add(profilePic);
                db.SaveChanges();
                return RedirectToAction("Index", "Home");
            }

            return View(profilePic);
        }



    }
}