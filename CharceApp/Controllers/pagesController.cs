using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CharceApp.Models;
using Microsoft.AspNet.Identity;

namespace CharceApp.Controllers
{
    public class pagesController : Controller
    {
        ApplicationDbContext db = new ApplicationDbContext();

        [Authorize]
        public ActionResult ViewOrder(int id)
        {
            Order orders = db.orders.ToList().Where(x => x.ID == id).FirstOrDefault();

            return View(orders);
        }


        [Authorize]
        public ActionResult BulkOrder(int business_id, int personal_id)
        {
            ListOrder listorder = db.listorders.ToList()
                .Where(x => x.BusinessID == business_id && x.PersonalAccID == personal_id)
                .FirstOrDefault();
            List<ProductListOrder> productorder = db.productlistorders.ToList()
                .Where(x => x.ListOrderID == listorder.ID).ToList();

            List<Order> orders = new List<Order>();
            double total=0;

            foreach(ProductListOrder p in productorder)
            {
                Order order = db.orders.ToList().Where(x => x.ID == p.OrderID).FirstOrDefault();
                if(order != null)
                {
                    orders.Add(order);
                    total += order.Price * order.Qauntity;
                }
            }
            ViewBag.OrderID = listorder.ID;
            ViewBag.Status = listorder.Status;
            ViewBag.Total = total;

            return View(orders);
        }


        [Authorize]
        public ActionResult OpenMessage(int RecieverID)//Accessed from profile
        {

            //If a conversation between the both of us exists, redirect me to Message( ), else show me a textbox that will call SendMessage( )
            string myUsername = User.Identity.GetUserName();
            string myId = User.Identity.GetUserId();

            ActiveProfile active = db.activeprofiles.ToList()
                .Where(x => x.ApplicationUserId == myId).FirstOrDefault();

            BusinessAccount ba = db.businessaccounts.ToList()
                .Where(x => x.ID == RecieverID).FirstOrDefault();
            PersonalAccount pa = db.personalaccounts.ToList().Where(x => x.AppUserId == myId).FirstOrDefault();

            ViewBag.MyID = active.ActiveProfileID;
            ViewBag.RecieverID = RecieverID;
            ViewBag.RecieverDisplayName = ba.BusinessName;
            Conversation convo = db.conversations.ToList().
                Where(x => (x.FirstPersonID == pa.ID || x.SecondPersonID == pa.ID) && (x.FirstPersonID == RecieverID || x.SecondPersonID == RecieverID))
                .FirstOrDefault();
            if (convo != null)
            {
                return RedirectToAction("Message", "pages", new { pageNo = 0, convoID = convo.ID });
            }
            else
            {
                return View(); //For creating the very first text or conversation, form must call SendMessage( )
            }

        }


        [Authorize]
        public ActionResult Message(int convoID, int pageNo)//List of messages within a conversation (accessed from Inbox page)
        {
            
            Conversation convo = db.conversations.ToList().Where(x => x.ID == convoID).FirstOrDefault();
            int page = pageNo;
            int next = page + 15;
            ViewBag.NextPosts = next;
            //string myUsername = User.Identity.GetUserName();
            string myId = User.Identity.GetUserId();
            ActiveProfile active = db.activeprofiles.ToList()
                .Where(x => x.ApplicationUserId == myId).FirstOrDefault();
            ViewBag.MyId = active.ActiveProfileID;
            ViewBag.ConvoID = convo.ID;
            int recieverid;
            bool im_business;

            if (active.AccountType == "Business")
            {
                im_business = true;
            }
            else
            {
                im_business = false;
            }

            ViewBag.isBusiness = im_business;
            

            if (convo.FirstPersonID == active.ActiveProfileID)
            {
                recieverid = convo.SecondPersonID;
            }
            else
            {
                recieverid = convo.FirstPersonID;
            }
            ViewBag.RecieverID = recieverid;

            if (convo != null && (convo.FirstPersonID == active.ActiveProfileID || convo.SecondPersonID == active.ActiveProfileID))
            {
                List<Message> messages = db.messages.ToList().Where(x => x.ConversationID == convo.ID).ToList();

                if (convo.LastSenderID != active.ActiveProfileID)
                {
                    convo.Seen = true;
                    db.SaveChanges();

                }
                //if (messages.Last().SenderID != active.ActiveProfileID && messages.Last().Seen == false)
                //{
                //    messages.Last().Seen = true;
                //    db.SaveChanges();
                //}

                return View(messages.Skip(page).Take(15));

            }
            else
            {
                return RedirectToAction("Index", "Home");
            }


        }






        [Authorize]
        public ActionResult AddAccount()
        {
            return View();
        }
       
        [Authorize]
        public ActionResult MyAccounts()
        {
            string myId = User.Identity.GetUserId();
            PersonalAccount pa = db.personalaccounts.ToList().Where(x => x.AppUserId == myId).FirstOrDefault();
            List<BusinessAccount> bussiness_accounts = db.businessaccounts.ToList()
                .Where(x => x.PersonalAccountID == pa.ID).ToList();


            
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

                foreach (BusinessAccount ba in b_accounts)
                {
                    if (ba.ID == active_profile.ActiveProfileID)
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



            return View(bussiness_accounts);
        }

        [Authorize]
        public ActionResult CheckoutView()
        {
            string myId = User.Identity.GetUserId();
            PersonalAccount p_acc = db.personalaccounts.ToList().Where(x => x.AppUserId == myId).FirstOrDefault();
            List<Cart> carts = db.carts.ToList().Where(x => x.PersonalAccountID == p_acc.ID).ToList();
            double balance = 0;
            foreach (Cart c in carts)
            {
                double price = c.Price * c.Qty;
                
                balance += price;
            }

            ViewBag.Balance = balance;
            return View(carts);
        }

        [Authorize]
        public ActionResult ViewAccount(string acc_type,int id)
        {
            string myId = User.Identity.GetUserId();
            PersonalAccount p_acc = db.personalaccounts.ToList()
                .Where(x => x.AppUserId == myId).FirstOrDefault();
            if (acc_type == "Business")
            {
                BusinessAccount b_acc = db.businessaccounts.ToList()
                    .Where(x => x.PersonalAccountID == p_acc.ID && x.ID==id).FirstOrDefault();

                ViewBag.AccountType = "Business";
                ViewBag.BusinessID = b_acc.ID;
                ViewBag.PersonalID = p_acc.ID;
                ViewBag.Name = b_acc.BusinessName;
                ViewBag.Location = b_acc.Location;
                ViewBag.Phone = b_acc.Phone;
                ViewBag.Email = b_acc.Email;
                ViewBag.Website = b_acc.Website;

                return View();
            }
            else{

                ViewBag.AccountType = "Personal";
                ViewBag.PersonalID = p_acc.ID;
                ViewBag.Name = p_acc.Names + " " + p_acc.Surname;
                ViewBag.Phone = p_acc.PhoneNumber;
                ViewBag.Email = User.Identity.GetUserName();

                return View();

            }

        }
    }
}