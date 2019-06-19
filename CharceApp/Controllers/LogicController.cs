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
        public ActionResult SendMessage(int RecieverID, string txt)
        {
            string myUsername = User.Identity.GetUserName();
            string myId = User.Identity.GetUserId();
            PersonalAccount pa = db.personalaccounts.ToList().Where(x => x.AppUserId == myId).FirstOrDefault();
            ActiveProfile active_profile = db.activeprofiles.ToList()
                .Where(x => x.ApplicationUserId == myId).FirstOrDefault();
            int active_id = active_profile.ActiveProfileID;

            Conversation convo = db.conversations.ToList().
                Where(x => (x.FirstPersonID == active_id || x.SecondPersonID == active_id) && (x.FirstPersonID == RecieverID || x.SecondPersonID == RecieverID))
                .FirstOrDefault();

            if (active_profile.AccountType == "Business") //meaning i am the business
            {
                BusinessAccount ba = db.businessaccounts.ToList()
                    .Where(x => x.PersonalAccountID == pa.ID && x.ID == active_profile.ActiveProfileID)
                    .FirstOrDefault();

                //If i am the business, then the other person must be the client/customer....
                PersonalAccount customer = db.personalaccounts.ToList()
                    .Where(x => x.ID == RecieverID).FirstOrDefault(); 
                //              ^^^^^^Customer^^^^^
                
                if (convo == null) //first time message
                {
                    Conversation conversation = new Conversation()
                    {
                        Date = DateTime.Now,
                        FirstPersonDispName = ba.BusinessName,
                        FirstPersonID = ba.ID,
                        SecondPersonDispName = customer.Names +" "+customer.Surname,
                        SecondPersonID = customer.ID,
                        LastMessage = txt,
                        LastSenderID = ba.ID,
                        Seen = false
                    };
                    db.conversations.Add(conversation);
                    db.SaveChanges();

                    Message msg = new Message()
                    {
                        Date = DateTime.Now,
                        ConversationID = conversation.ID,
                        SenderDispName = ba.BusinessName,
                        SenderID = ba.ID,
                        Seen = false,
                        Text = txt
                    };
                    db.messages.Add(msg);
                    db.SaveChanges();

                    NewMessageNotification nmn = new NewMessageNotification()
                    {
                        RecieverID = RecieverID
                    };
                    db.newmessagenotifications.Add(nmn);
                    db.SaveChanges();

                    return RedirectToAction("Message", "pages", new { pageNo = 0, convoID = conversation.ID });
                }
                else //meaning this is not the first text
                {
                    


                    Message msg = new Message()
                    {
                        Date = DateTime.Now,
                        ConversationID = convo.ID,
                        SenderDispName = ba.BusinessName,
                        SenderID = ba.ID,
                        Seen = false,
                        Text = txt
                    };
                    db.messages.Add(msg);
                    db.SaveChanges();

                    NewMessageNotification nmn = new NewMessageNotification()
                    {
                        RecieverID = RecieverID
                    };
                    db.newmessagenotifications.Add(nmn);
                    db.SaveChanges();

                    convo.LastSenderID = active_id;
                    convo.LastMessage = txt;
                    convo.Date = DateTime.Now;
                    convo.Seen = false;
                    db.SaveChanges();

                    return RedirectToAction("Message", "pages", new { pageNo = 0, convoID = convo.ID });
                }

            }
            else // I am the customer (object pa ^^^^), sending a message to the business
            {

                BusinessAccount company = db.businessaccounts.ToList()
                    .Where(x => x.ID == RecieverID).FirstOrDefault();


                if (convo == null) //first time message
                {
                    Conversation conversation = new Conversation()
                    {
                        Date = DateTime.Now,
                        FirstPersonDispName = pa.Names+" "+pa.Surname,
                        FirstPersonID = pa.ID,
                        SecondPersonDispName = company.BusinessName,
                        SecondPersonID = company.ID,
                        LastMessage = txt,
                        LastSenderID = pa.ID,
                        Seen = false
                    };
                    db.conversations.Add(conversation);
                    db.SaveChanges();

                    Message msg = new Message()
                    {
                        Date = DateTime.Now,
                        ConversationID = conversation.ID,
                        SenderDispName = pa.Names + " " + pa.Surname,
                        SenderID = pa.ID,
                        Seen = false,
                        Text = txt
                    };
                    db.messages.Add(msg);
                    db.SaveChanges();

                    NewMessageNotification nmn = new NewMessageNotification()
                    {
                        RecieverID = RecieverID
                    };
                    db.newmessagenotifications.Add(nmn);
                    db.SaveChanges();

                    return RedirectToAction("Message", "pages", new { pageNo = 0, convoID = conversation.ID });
                }
                else //meaning this is not the first text
                {



                    Message msg = new Message()
                    {
                        Date = DateTime.Now,
                        ConversationID = convo.ID,
                        SenderDispName = pa.Names + " " + pa.Surname,
                        SenderID = pa.ID,
                        Seen = false,
                        Text = txt
                    };
                    db.messages.Add(msg);
                    db.SaveChanges();

                    NewMessageNotification nmn = new NewMessageNotification()
                    {
                        RecieverID = RecieverID
                    };
                    db.newmessagenotifications.Add(nmn);
                    db.SaveChanges();

                    convo.LastSenderID = active_id;
                    convo.LastMessage = txt;
                    convo.Date = DateTime.Now;
                    convo.Seen = false;
                    db.SaveChanges();

                    return RedirectToAction("Message", "pages", new { pageNo = 0, convoID = convo.ID });
                }

            }

        }


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