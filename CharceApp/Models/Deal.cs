using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CharceApp.Models
{
    public class Deal
    {
        public int ID { get; set; }
        public int BusinessID { get; set; }
        public int ProductID { get; set; }

        public double Price { get; set; }
        public double PreviousPrice { get; set; }
        public string Description { get; set; }
    }
}