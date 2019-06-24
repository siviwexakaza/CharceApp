using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CharceApp.Models
{
    public class ListOrder
    {
        public int ID { get; set; }
        public int BusinessID { get; set; }
        public int PersonalAccID { get; set; }
        public DateTime Date { get; set; }
        public string Status { get; set; }

    }
}