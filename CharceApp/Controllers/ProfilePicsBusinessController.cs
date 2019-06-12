using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CharceApp.Models;

namespace CharceApp.Controllers
{
    public class ProfilePicsBusinessController : Controller
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
        public ActionResult Create([Bind(Include = "ID,BusinessId")] ProfilePic_Business profilePic, HttpPostedFileBase pic, int BusinessID)
        {
           // string myId = User.Identity.GetUserId();
           // PersonalAccount p_acc = db.personalaccounts.ToList().Where(x => x.AppUserId == myId).FirstOrDefault();

            profilePic.BusinessId = BusinessID;


            if (ModelState.IsValid)
            {
                if (pic != null && pic.ContentLength > 0)
                {

                    var avatar = new File_Business
                    {

                        FileName = System.IO.Path.GetFileName(pic.FileName),
                        FileType_Business = FileType_Business.Avatar,
                        ContentType = pic.ContentType,
                        ProfilePic_BusinessID= BusinessID,
                        BusinessId=BusinessID
                    };

                    using (var reader = new System.IO.BinaryReader(pic.InputStream))
                    {
                        avatar.Content = reader.ReadBytes(pic.ContentLength);
                    }
                    profilePic.Files_Business = new List<File_Business> { avatar };
                }
                List<ProfilePic_Business> pics = db.profilepic_businesses.ToList().Where(x => x.BusinessId == BusinessID).ToList();
                if (pics != null)
                {
                    foreach (File_Business file in db.File_Businesses.ToList())
                    {
                        foreach (ProfilePic_Business pro in pics)
                        {
                            if (file.ProfilePic_BusinessID == pro.ID)
                            {
                                db.File_Businesses.Remove(file);
                            }
                        }
                    }

                    foreach (ProfilePic_Business pp in pics)
                    {
                        db.profilepic_businesses.Remove(pp);
                    }
                }
                db.profilepic_businesses.Add(profilePic);
                db.SaveChanges();
                return RedirectToAction("Index", "Home");
            }

            return View(profilePic);
        }






        //ApplicationDbContext db = new ApplicationDbContext();

        //public ActionResult Index(int id)
        //{
        //    var pic = db.Files.FirstOrDefault(x => x.ProfilePicID == id);
        //    return File(pic.Content, pic.ContentType);
        //}
    }
}