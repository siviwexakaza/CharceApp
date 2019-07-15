using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CharceApp.Models;
using Microsoft.AspNet.Identity;

namespace CharceApp.Controllers
{
    public class HomeController : Controller
    {
        ApplicationDbContext db = new ApplicationDbContext();

        [Authorize]
        public ActionResult Index(string search)
        {
            
            string myId = User.Identity.GetUserId();
            ActiveProfile active_profile = db.activeprofiles.ToList().
                Where(x => x.ApplicationUserId == myId).FirstOrDefault();

            bool personal_account_is_active = false;
            string account_name = "";
            string account_type = "";
            int account_id;
            

            PersonalAccount personal_account = db.personalaccounts.ToList().
                Where(x => x.AppUserId == myId).FirstOrDefault();
            if (personal_account.ID == active_profile.ActiveProfileID)
            {
                personal_account_is_active = true;
            }
            else
            {
                personal_account_is_active = false;
            }

            if (personal_account_is_active)
            {
                account_name = personal_account.Names + " " + personal_account.Surname;
                account_type = active_profile.AccountType;
                account_id = personal_account.ID;
            }
            else
            {
                BusinessAccount business_account = new BusinessAccount();
                List<BusinessAccount> b_accounts = db.businessaccounts.ToList().
                    Where(x => x.PersonalAccountID == personal_account.ID).ToList();

                foreach(BusinessAccount ba in b_accounts)
                {
                    if(ba.ID == active_profile.ActiveProfileID)
                    {
                        business_account = ba;
                        break;
                    }
                }

                account_name = business_account.BusinessName;
                account_type = active_profile.AccountType;
                account_id = business_account.ID;
                string business_type = business_account.BusinessType;


            }
            ViewBag.isPersonalAccount = personal_account_is_active;
            ViewBag.AccountName = account_name;
            ViewBag.AccountID = account_id;
            ViewBag.AccountType = account_type;
            ViewBag.Search = search;

            bool hasSearch = false;

            if(search != null)
            {
                List<Conversation> conversations = db.conversations.ToList()
                .Where(x =>( x.FirstPersonID == account_id || x.SecondPersonID == account_id)
                && (x.LastMessage.ToLower().Contains(search.ToLower()) || x.FirstPersonDispName.ToLower().Contains(search.ToLower()) ||
                x.SecondPersonDispName.ToLower().Contains(search.ToLower()))).ToList();
                hasSearch = true;
                ViewBag.hasSearch = hasSearch;
                

                return View(conversations.OrderByDescending(x => x.Date));
            }
            else
            {
                List<Conversation> conversations = db.conversations.ToList()
                .Where(x => x.FirstPersonID == account_id || x.SecondPersonID == account_id).ToList();
                ViewBag.hasSearch = hasSearch;

                return View(conversations.OrderByDescending(x => x.Date));
            }
            


            
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}