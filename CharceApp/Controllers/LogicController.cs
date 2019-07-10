﻿using CharceApp.Models;
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
        public ActionResult CreateDeal(int prod_id,double price, string descr)
        {
            string myid = User.Identity.GetUserId();
            ActiveProfile act = db.activeprofiles.ToList()
                .Where(x => x.ApplicationUserId == myid).FirstOrDefault();

            ProfilePic_Product pro = db.profilepic_products.ToList()
                .Where(x => x.ID == prod_id && x.BusinessId==act.ActiveProfileID).FirstOrDefault();

            Deal deal = new Deal() {
                BusinessID=act.ActiveProfileID, Description=descr,ProductID=prod_id,
                Price=price, PreviousPrice=pro.Price
            };

            db.deals.Add(deal);
            pro.Price = price;
            db.SaveChanges();

            return RedirectToAction("Deals", "Pages");
        }

        [Authorize]
        public ActionResult DeleteDeal(int id)
        {
            string myid = User.Identity.GetUserId();
            ActiveProfile act = db.activeprofiles.ToList()
                .Where(x => x.ApplicationUserId == myid).FirstOrDefault();
            Deal deal = db.deals.ToList()
                .Where(x => x.ID == id && x.BusinessID == act.ActiveProfileID).FirstOrDefault();

            if(deal == null)
            {
                return Redirect(Request.UrlReferrer.ToString());
            }
            ProfilePic_Product product = db.profilepic_products.ToList()
                .Where(x => x.ID == deal.ProductID).FirstOrDefault();
            product.Price = deal.PreviousPrice;
            db.deals.Remove(deal);
            db.SaveChanges();
            return Redirect(Request.UrlReferrer.ToString());
        }

        [Authorize]
        public ActionResult ToggleFollow(int business_id)
        {
            BusinessAccount b = db.businessaccounts.ToList().Where(x => x.ID == business_id).FirstOrDefault();
            string myid = User.Identity.GetUserId();
            PersonalAccount p = db.personalaccounts.ToList().Where(x => x.AppUserId == myid).FirstOrDefault();

            Follow follows = db.follows.ToList().Where(x => x.BusinessID == b.ID && x.PersonalAccID == p.ID).FirstOrDefault();

            if(follows != null)
            {
                db.follows.Remove(follows);
                db.SaveChanges();
            }
            else
            {
                Follow follow = new Follow() {
                    BusinessID=b.ID, PersonalAccID=p.ID
                };

                db.follows.Add(follow);
                db.SaveChanges();

            }

            return Redirect(Request.UrlReferrer.ToString());
        }

        [Authorize]
        public void SeenMessage()
        {
            string myid = User.Identity.GetUserId();
            ActiveProfile act = db.activeprofiles.ToList()
                .Where(x => x.ApplicationUserId == myid).FirstOrDefault();

            if (act.AccountType == "Business")
            {
                ChatScreen c = db.chatscreens.ToList()
                    .Where(x => x.AccountType == "Business" && x.AccountID == act.ActiveProfileID)
                    .FirstOrDefault();
                c.hasMessage = false;
                db.SaveChanges();
            }
            else
            {
                ChatScreen c = db.chatscreens.ToList()
                    .Where(x => x.AccountType == "Personal" && x.AccountID == act.ActiveProfileID)
                    .FirstOrDefault();
                c.hasMessage = false;
                db.SaveChanges();
            }

        }

        [Authorize]
        public JsonResult CheckNewMessage()
        {
            string myid = User.Identity.GetUserId();
            ActiveProfile act = db.activeprofiles.ToList()
                .Where(x => x.ApplicationUserId == myid).FirstOrDefault();
            if (act.AccountType == "Business")
            {
                BusinessAccount b = db.businessaccounts.ToList()
                    .Where(x => x.ID == act.ActiveProfileID).FirstOrDefault();

                ChatScreen chats = db.chatscreens.ToList()
                    .Where(x => x.AccountType == "Business" && x.AccountID == b.ID).FirstOrDefault();
                bool hasMessage=false;

                if(chats != null)
                {
                    if (chats.hasMessage)
                    {
                        hasMessage = true;
                       // chats.hasMessage = false;
                        db.SaveChanges();
                    }
                    else
                    {
                        hasMessage = false;
                    }
                    
                }
                else
                {
                    ChatScreen c = new ChatScreen() {
                        hasMessage = false, AccountType="Business",AccountID=b.ID
                    };
                    db.chatscreens.Add(c);
                    db.SaveChanges();
                }

                ReloadScreenVM r = new ReloadScreenVM() { reload=hasMessage};

                return Json(r, JsonRequestBehavior.AllowGet);
            }
            else
            {
                PersonalAccount p = db.personalaccounts.ToList()
                    .Where(x => x.AppUserId == myid).FirstOrDefault();

                ChatScreen chats = db.chatscreens.ToList()
                    .Where(x => x.AccountType == "Personal" && x.AccountID == p.ID).FirstOrDefault();

                bool hasMessage = false;

                if (chats != null)
                {
                    if (chats.hasMessage)
                    {
                        hasMessage = true;
                       // chats.hasMessage = false;
                        db.SaveChanges();
                    }
                    else
                    {
                        hasMessage = false;
                    }

                }
                else
                {
                    ChatScreen c = new ChatScreen()
                    {
                        hasMessage = false,
                        AccountType = "Personal",
                        AccountID = p.ID
                    };
                    db.chatscreens.Add(c);
                    db.SaveChanges();
                }

                ReloadScreenVM r = new ReloadScreenVM() { reload = hasMessage };

                return Json(r, JsonRequestBehavior.AllowGet);
            }
        }

        [Authorize]
        public ActionResult ChangeOrderStatus(int listorderid, string newstatus)
        {
            ListOrder order = db.listorders.ToList().Where(x => x.ID == listorderid).FirstOrDefault();
            order.Status = newstatus;
            
            db.SaveChanges();

            SendMessage(order.PersonalAccID, order.Date.TimeOfDay.ToString().Substring(0, 5)+ "Order #" + order.ID + " is " + newstatus,0);

            return Redirect(Request.UrlReferrer.ToString());

        }

        [Authorize]
        public ActionResult SendMessage(int RecieverID, string txt, int orderid)
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

                ChatScreen chat = db.chatscreens.ToList()
                    .Where(x => x.AccountType == "Personal" && x.AccountID == customer.ID).FirstOrDefault();
                if(chat != null)
                {
                    chat.hasMessage = true;
                    db.SaveChanges();
                }
                else
                {
                    ChatScreen c = new ChatScreen() {
                        AccountType="Personal", AccountID=customer.ID,hasMessage=true
                    };
                    db.chatscreens.Add(c);
                    db.SaveChanges();
                }
                
                if (convo == null) //first time message
                {
                    if (txt.Substring(2, 1) == ":")
                    {
                        Conversation conversation = new Conversation()
                        {
                            Date = DateTime.Now,
                            FirstPersonDispName = ba.BusinessName,
                            FirstPersonID = ba.ID,
                            SecondPersonDispName = customer.Names + " " + customer.Surname,
                            SecondPersonID = customer.ID,
                            LastMessage = txt.Substring(5),
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
                            Text = txt,
                            isOrder = false,
                            OrderID = 0
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
                    else
                    {
                        Conversation conversation = new Conversation()
                        {
                            Date = DateTime.Now,
                            FirstPersonDispName = ba.BusinessName,
                            FirstPersonID = ba.ID,
                            SecondPersonDispName = customer.Names + " " + customer.Surname,
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
                            Text = txt,
                            isOrder = false,
                            OrderID = 0
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



                    
                }
                else //meaning this is not the first text
                {
                    if (txt.Substring(2, 1) == ":")
                    {
                        Message msg = new Message()
                        {
                            Date = DateTime.Now,
                            ConversationID = convo.ID,
                            SenderDispName = ba.BusinessName,
                            SenderID = ba.ID,
                            Seen = false,
                            Text = txt,
                            isOrder = false,
                            OrderID = 0
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
                        convo.LastMessage = txt.Substring(5);
                        convo.Date = DateTime.Now;
                        convo.Seen = false;
                        db.SaveChanges();

                        return RedirectToAction("Message", "pages", new { pageNo = 0, convoID = convo.ID });
                    }
                    else
                    {
                        Message msg = new Message()
                        {
                            Date = DateTime.Now,
                            ConversationID = convo.ID,
                            SenderDispName = ba.BusinessName,
                            SenderID = ba.ID,
                            Seen = false,
                            Text = txt,
                            isOrder = false,
                            OrderID = 0
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
            else // I am the customer (object pa ^^^^), sending a message to the business
            {

                BusinessAccount company = db.businessaccounts.ToList()
                    .Where(x => x.ID == RecieverID).FirstOrDefault();

                ChatScreen chat = db.chatscreens.ToList()
                    .Where(x => x.AccountType == "Business" && x.AccountID == company.ID).FirstOrDefault();
                if (chat != null)
                {
                    chat.hasMessage = true;
                    db.SaveChanges();
                }
                else
                {
                    ChatScreen c = new ChatScreen()
                    {
                        AccountType = "Business",
                        AccountID = company.ID,
                        hasMessage = true
                    };
                    db.chatscreens.Add(c);
                    db.SaveChanges();
                }


                if (convo == null) //first time message
                {

                    if(orderid != 0)
                    {
                        Order order = db.orders.ToList().Where(x => x.ID == orderid).FirstOrDefault();

                        Conversation conversation = new Conversation()
                        {
                            Date = DateTime.Now,
                            FirstPersonDispName = pa.Names + " " + pa.Surname,
                            FirstPersonID = pa.ID,
                            SecondPersonDispName = company.BusinessName,
                            SecondPersonID = company.ID,
                            LastMessage = pa.Names + " " + pa.Surname +" "+"placed an order for"+order.Qauntity+" "+order.Product,
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
                            Text = conversation.LastMessage
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
                    else
                    {
                        Conversation conversation = new Conversation()
                        {
                            Date = DateTime.Now,
                            FirstPersonDispName = pa.Names + " " + pa.Surname,
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


                    
                }
                else //meaning this is not the first text
                {

                    if(orderid != 0)
                    {
                        Order order = db.orders.ToList().Where(x => x.ID == orderid).FirstOrDefault();

                        Message msg = new Message()
                        {
                            Date = DateTime.Now,
                            ConversationID = convo.ID,
                            SenderDispName = pa.Names + " " + pa.Surname,
                            SenderID = pa.ID,
                            Seen = false,
                            Text = pa.Names + " " + pa.Surname + " " + "placed an order for" + order.Qauntity + " " + order.Product
                        };
                        db.messages.Add(msg);
                        db.SaveChanges();

                        NewMessageNotification nmn = new NewMessageNotification()
                        {
                            RecieverID = RecieverID
                        };
                        db.newmessagenotifications.Add(nmn);
                        db.SaveChanges();

                        return RedirectToAction("Message", "pages", new { pageNo = 0, convoID = convo.ID });

                    }
                    else
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

        }


        [Authorize]
        public ActionResult createOrder(string paymentmethod, string shippingmethod, string shippingaddress)
        {
            string myId = User.Identity.GetUserId();
            PersonalAccount pa = db.personalaccounts.ToList().Where(x => x.AppUserId == myId).FirstOrDefault();
            ActiveProfile active_profile = db.activeprofiles.ToList()
                .Where(x => x.ApplicationUserId == myId).FirstOrDefault();


            List<Cart> carts = db.carts.ToList()
                .Where(x => x.PersonalAccountID == active_profile.ActiveProfileID).ToList();

            List<int> businessids = new List<int>();

            foreach(Cart c in carts)
            {
                Order order = new Order() {
                    SendTo= shippingaddress,
                    CartID=c.ID, BusinessID=c.BusinessID,
                    Date=DateTime.Now,DeliverOrCollect= shippingmethod,
                    PaymentMethod= paymentmethod,
                    PersonalAccID=active_profile.ActiveProfileID, Price=c.Price,
                    Product=c.Name,ProductID=c.ProfilePicProductID,Qauntity=c.Qty,
                    Total=c.Price * c.Qty
                };

                if (!businessids.Contains(c.BusinessID))
                {
                    businessids.Add(c.BusinessID);
                }

                db.orders.Add(order);
                db.SaveChanges();

                ListOrder list = db.listorders.ToList()
                    .Where(x => x.BusinessID == order.BusinessID && x.PersonalAccID == order.PersonalAccID &&
                    x.Status !="Seen" && x.Status !="Accepted" && x.Status !="Declined" && x.Status != "Pending"
                    && x.Status != "Shipped" && x.Status != "Delivered" && x.Status != "Ready to Pickup" && x.Status != "Returned")
                    .FirstOrDefault();
                if(list == null)
                {
                    ListOrder l = new ListOrder()
                    {
                        BusinessID= order.BusinessID,
                        PersonalAccID =order.PersonalAccID,
                        Date=DateTime.Now,
                        Status="Pending"
                    };

                    db.listorders.Add(l);
                    db.SaveChanges();

                    ProductListOrder pro = new ProductListOrder() {
                        ListOrderID=l.ID, OrderID= order.ID
                    };

                    db.productlistorders.Add(pro);
                    db.SaveChanges();
                    SendOrder(c.BusinessID, "New Order", order.ID,l.ID);
                }
                else
                {
                    ProductListOrder pro = new ProductListOrder()
                    {
                        ListOrderID = list.ID,
                        OrderID = order.ID
                    };

                    db.productlistorders.Add(pro);
                    db.SaveChanges();

                    SendOrder(c.BusinessID, "New Order", order.ID, list.ID);

                }

                
                
            }
            SendOrderMessage(businessids);

            return RedirectToAction("Index", "Home");

        }

        [Authorize]
        public void SendOrderMessage(List<int> businessids)
        {
            string myId = User.Identity.GetUserId();
            PersonalAccount pa = db.personalaccounts.ToList()
                .Where(x => x.AppUserId == myId).FirstOrDefault();

            foreach(int id in businessids)
            {
                Conversation convo = db.conversations.ToList().
                Where(x => (x.FirstPersonID == pa.ID || x.SecondPersonID == pa.ID) && (x.FirstPersonID == id || x.SecondPersonID == id))
                .FirstOrDefault();


                BusinessAccount b = db.businessaccounts.ToList().Where(x => x.ID == id).FirstOrDefault();

                ChatScreen chat = db.chatscreens.ToList()
                    .Where(x => x.AccountType == "Business" && x.AccountID == b.ID).FirstOrDefault();
                if (chat != null)
                {
                    chat.hasMessage = true;
                    db.SaveChanges();
                }
                else
                {
                    ChatScreen c = new ChatScreen()
                    {
                        AccountType = "Business",
                        AccountID = b.ID,
                        hasMessage = true
                    };
                    db.chatscreens.Add(c);
                    db.SaveChanges();
                }


                if (convo == null)
                {
                    

                    Conversation conversation = new Conversation()
                    {
                        Date = DateTime.Now,
                        FirstPersonDispName = pa.Names + " " + pa.Surname,
                        FirstPersonID = pa.ID,
                        SecondPersonDispName = b.BusinessName,
                        SecondPersonID = b.ID,
                        LastMessage = pa.Names + " " + pa.Surname + " " + "placed an order",
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
                        Text = "New order has been placed",
                        isOrder = true,
                        OrderID = 0
                    };
                    db.messages.Add(msg);
                    db.SaveChanges();

                    NewMessageNotification nmn = new NewMessageNotification()
                    {
                        RecieverID = id
                    };
                    db.newmessagenotifications.Add(nmn);
                    db.SaveChanges();


                }
                else
                {
                    Message msg = new Message()
                    {
                        Date = DateTime.Now,
                        ConversationID = convo.ID,
                        SenderDispName = pa.Names + " " + pa.Surname,
                        SenderID = pa.ID,
                        Seen = false,
                        Text = "New order has been placed",
                        isOrder = true,
                        OrderID = 0
                    };
                    db.messages.Add(msg);
                    db.SaveChanges();
                    convo.LastMessage = pa.Names + " " + pa.Surname + " " + "placed an order";
                    convo.LastSenderID = pa.ID;
                    convo.Date = DateTime.Now;
                    convo.Seen = false;


                    NewMessageNotification nmn = new NewMessageNotification()
                    {
                        RecieverID = id
                    };
                    db.newmessagenotifications.Add(nmn);

                    db.SaveChanges();

                }

            }

        }
        


        [Authorize]
        public void SendOrder(int RecieverID, string txt, int orderid,int listorder_id)
        {
            string myUsername = User.Identity.GetUserName();
            string myId = User.Identity.GetUserId();
            PersonalAccount pa = db.personalaccounts.ToList().Where(x => x.AppUserId == myId).FirstOrDefault();
            ActiveProfile active_profile = db.activeprofiles.ToList()
                .Where(x => x.ApplicationUserId == myId).FirstOrDefault();
            int active_id = active_profile.ActiveProfileID;

                BusinessAccount company = db.businessaccounts.ToList()
                    .Where(x => x.ID == RecieverID).FirstOrDefault();

                    if (orderid != 0)
                    {
                        Order order = db.orders.ToList().Where(x => x.ID == orderid).FirstOrDefault();
                        Cart cart = db.carts.ToList().Where(x => x.ID == order.CartID).FirstOrDefault();

                        

                        ListOrder l_order = db.listorders.ToList()
                            .Where(x => x.ID == listorder_id && x.Status=="Pending").FirstOrDefault();

                        db.carts.Remove(cart);
                        db.SaveChanges();


                    }
                    else
                    {
                        return;

                        

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