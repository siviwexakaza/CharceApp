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