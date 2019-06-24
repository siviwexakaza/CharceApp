using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CharceApp.Models
{
    public class Order
    {
        public int ID { get; set; }
        public DateTime Date { get; set; }
        public int ProductID { get; set; }
        public int BusinessID { get; set; }
        public int CartID { get; set; }
        public int PersonalAccID { get; set; }
        public string Product { get; set; }
        public int Qauntity { get; set; }
        public string DeliverOrCollect { get; set; }
        public string PaymentMethod { get; set; }
        public double Price { get; set; }
        public double Total { get; set; }
        public string BillingAddress { get; set; }
    }
}