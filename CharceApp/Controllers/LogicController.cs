using CharceApp.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CharceApp.Controllers
{
    public class LogicController : Controller
    {
        ApplicationDbContext db = new ApplicationDbContext();


        [Authorize]
        public ActionResult ToggleCart(int productId, int businessId)
        {
            string myId = User.Identity.GetUserId();
            PersonalAccount p_acc = db.personalaccounts.ToList().Where(x => x.AppUserId == myId).FirstOrDefault();
            BusinessAccount b_acc = db.businessaccounts.ToList().Where(x => x.ID == businessId).FirstOrDefault();
            ProfilePic_Product prod = db.profilepic_products.ToList()
                .Where(x => x.ID == productId && x.BusinessId == businessId).FirstOrDefault();
            Cart cart = db.carts.ToList()
                .Where(x => x.PersonalAccountID == p_acc.ID && x.ProfilePicProductID == prod.ID && x.BusinessID==businessId)
                .FirstOrDefault();

            if (cart == null)
            {
                Cart c = new Cart() {
                    BusinessID=businessId, PersonalAccountID=p_acc.ID,
                    ProfilePicProductID=prod.ID,Name=prod.Name,
                    Description=prod.Description,ItemType=prod.Item_Type,
                    Price=prod.Price,Qty=1
                };
                db.carts.Add(c);
                db.SaveChanges();

            }
            else
            {
                db.carts.Remove(cart);
                db.SaveChanges();
            }

            return Redirect(Request.UrlReferrer.ToString());



        }

        [Authorize]
        public ActionResult IncreaseQty(int cartId)
        {
            Cart c = db.carts.ToList().Where(x => x.ID == cartId).FirstOrDefault();
            c.Qty++;
            db.SaveChanges();
            return Redirect(Request.UrlReferrer.ToString());

        }

        [Authorize]
        public ActionResult DecreaseQty(int cartId)
        {
            Cart c = db.carts.ToList().Where(x => x.ID == cartId).FirstOrDefault();
            c.Qty--;
            db.SaveChanges();
            return Redirect(Request.UrlReferrer.ToString());

        }

        [Authorize]
        public void SwitchAccount(int BusinessAccID)
        {
            string myId = User.Identity.GetUserId();
            PersonalAccount pa = db.personalaccounts.ToList().Where(x => x.AppUserId == myId).FirstOrDefault();
            List<BusinessAccount> bussiness_accounts = db.businessaccounts.ToList()
                .Where(x => x.PersonalAccountID == pa.ID).ToList();


            ActiveProfile active_profile = db.activeprofiles.ToList().
                Where(x => x.ApplicationUserId == myId).FirstOrDefault();

            foreach (BusinessAccount b in bussiness_accounts)
            {
                if (b.ID == BusinessAccID && BusinessAccID != active_profile.ActiveProfileID)
                {
                    active_profile.ActiveProfileID = BusinessAccID;
                    active_profile.AccountType = "Business";
                    db.SaveChanges();
                    break;
                }
                else if (b.ID == BusinessAccID && BusinessAccID == active_profile.ActiveProfileID)
                {
                    active_profile.ActiveProfileID = pa.ID;
                    active_profile.AccountType = "Personal";
                    db.SaveChanges();
                    break;
                }
                
            }
        }

    }
}