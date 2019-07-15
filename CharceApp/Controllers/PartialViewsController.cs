using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CharceApp.Models;
using Microsoft.AspNet.Identity;

namespace CharceApp.Controllers
{
    public class PartialViewsController : Controller
    {
        ApplicationDbContext db = new ApplicationDbContext();
        public ActionResult _DesktopMessages(int convo_id)
        {
            string myid = User.Identity.GetUserId();
            ActiveProfile act = db.activeprofiles.ToList().Where(x => x.ApplicationUserId == myid).FirstOrDefault();
            ViewBag.AccountType = act.AccountType;
            
            ViewBag.AccountID = act.ActiveProfileID;
            Conversation convo = db.conversations.ToList().Where(x => x.ID == convo_id).FirstOrDefault();
            if (convo.FirstPersonID == act.ActiveProfileID)
            {
                ViewBag.RecieverID = convo.SecondPersonID;
                ViewBag.RecieverName = convo.SecondPersonDispName;
            }
            else
            {
                ViewBag.RecieverID = convo.FirstPersonID;
                ViewBag.RecieverName = convo.FirstPersonDispName;
            }

            List<Message> msgs = db.messages.ToList().Where(x => x.ConversationID == convo_id).ToList();

            return PartialView(msgs);
        }
        
    }
}