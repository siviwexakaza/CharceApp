using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CharceApp.Models
{
    public class Cart
    {
        public int ID { get; set; }
        public int PersonalAccountID { get; set; }
        public int BusinessID { get; set; }
        public int ProfilePicProductID { get; set; }
        public int Qty { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public double Price { get; set; }
        public string ItemType { get; set; }
    }
}