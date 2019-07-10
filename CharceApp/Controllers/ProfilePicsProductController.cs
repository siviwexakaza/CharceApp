using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CharceApp.Models;
using Microsoft.AspNet.Identity;

namespace CharceApp.Controllers
{
    public class ProfilePicsProductController : Controller
    {
        ApplicationDbContext db = new ApplicationDbContext();
        public ActionResult Index(string search)
        {

            string myid = User.Identity.GetUserId();
            PersonalAccount p = db.personalaccounts.ToList().Where(x => x.AppUserId == myid).FirstOrDefault();
            int items = db.carts.ToList().Where(x => x.PersonalAccountID == p.ID).ToList().Count();
            ViewBag.Items = items;
            ViewBag.MyID = p.ID;

            ViewBag.Search = search;

            bool hasSearch = false;

            if (search != null)
            {
                hasSearch = true;
                ViewBag.hasSearch = hasSearch;
                List<ProfilePic_Product> pro = db.profilepic_products.ToList()
                    .Where(x=>x.Name !=null && x.Name.ToLower().Contains(search.ToLower())).ToList();
                
                return View(pro);

                
            }
            else
            {
                ViewBag.hasSearch = hasSearch;
                return View(db.profilepic_products.ToList().OrderByDescending(x => x.ID));
                
            }

            
        }

        [Authorize]
        public ActionResult CompanyProducts(int id)
        {
            string myid = User.Identity.GetUserId();
            PersonalAccount p = db.personalaccounts.ToList().Where(x => x.AppUserId == myid).FirstOrDefault();
            BusinessAccount business = db.businessaccounts.ToList().Where(x => x.ID == id).FirstOrDefault();

            if (business.PersonalAccountID == p.ID)
            {
                int items = db.carts.ToList().Where(x => x.PersonalAccountID == p.ID).ToList().Count();
                ViewBag.Items = items;
                ViewBag.MyID = p.ID;
                return View(db.profilepic_products.ToList()
                    .Where(x => x.BusinessId == id).OrderByDescending(x => x.ID));
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
            
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
        public ActionResult Create([Bind(Include = "ID,BusinessId")] string Name, string Description, string Item_Type,
            double Discount, double Price, double Tax , HttpPostedFileBase pic, int BusinessID)
        {
            // string myId = User.Identity.GetUserId();
            // PersonalAccount p_acc = db.personalaccounts.ToList().Where(x => x.AppUserId == myId).FirstOrDefault();
            ProfilePic_Product profilePic = new ProfilePic_Product();
            profilePic.BusinessId = BusinessID;
            profilePic.Name = Name;
            profilePic.Description = Description;
            profilePic.Item_Type = Item_Type;
            profilePic.Discount = Discount;
            profilePic.Price = Price;
            profilePic.Tax = Tax;


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