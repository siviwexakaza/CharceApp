using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CharceApp.Models
{
    public class ProductListOrder
    {
        public int ID { get; set; }
        public int OrderID { get; set; }
        public int ListOrderID { get; set; }
    }
}