using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using CharceApp.Models;
using Microsoft.AspNet.Identity;

namespace CharceApp.Models
{
    public class ProfilePicsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: ProfilePics
        public ActionResult Index()
        {
            return View(db.profilepics.ToList().OrderByDescending(p => p.ID));
        }

        // GET: ProfilePics/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProfilePic profilePic = db.profilepics.Find(id);
            if (profilePic == null)
            {
                return HttpNotFound();
            }
            return View(profilePic);
        }

        // GET: ProfilePics/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: ProfilePics/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,UserId")] ProfilePic profilePic, HttpPostedFileBase pic)
        {
            string myId = User.Identity.GetUserId();
            PersonalAccount p_acc = db.personalaccounts.ToList().Where(x => x.AppUserId == myId).FirstOrDefault();
            
            profilePic.UserId = p_acc.ID;


            if (ModelState.IsValid)
            {
                if (pic != null && pic.ContentLength > 0)
                {

                    var avatar = new File
                    {

                        FileName = System.IO.Path.GetFileName(pic.FileName),
                        FileType = FileType.Avatar,
                        ContentType = pic.ContentType
                    };

                    using (var reader = new System.IO.BinaryReader(pic.InputStream))
                    {
                        avatar.Content = reader.ReadBytes(pic.ContentLength);
                    }
                    profilePic.Files = new List<File> { avatar };
                }
                List<ProfilePic> pics = db.profilepics.ToList().Where(x => x.UserId == p_acc.ID).ToList();
                if (pics != null)
                {
                    foreach (File file in db.Files.ToList())
                    {
                        foreach (ProfilePic pro in pics)
                        {
                            if (file.ProfilePicID == pro.ID)
                            {
                                db.Files.Remove(file);
                            }
                        }
                    }

                    foreach (ProfilePic pp in pics)
                    {
                        db.profilepics.Remove(pp);
                    }
                }
                db.profilepics.Add(profilePic);
                db.SaveChanges();
                return RedirectToAction("personalpicuploaded", "pages",new { personal_id=p_acc.ID, pic_id=profilePic.ID });
            }

            return View(profilePic);
        }

        // GET: ProfilePics/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProfilePic profilePic = db.profilepics.Find(id);
            if (profilePic == null)
            {
                return HttpNotFound();
            }
            return View(profilePic);
        }

        // POST: ProfilePics/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,UserId")] ProfilePic profilePic)
        {
            if (ModelState.IsValid)
            {
                db.Entry(profilePic).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(profilePic);
        }

        // GET: ProfilePics/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProfilePic profilePic = db.profilepics.Find(id);
            if (profilePic == null)
            {
                return HttpNotFound();
            }
            return View(profilePic);
        }

        // POST: ProfilePics/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ProfilePic profilePic = db.profilepics.Find(id);
            db.profilepics.Remove(profilePic);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
