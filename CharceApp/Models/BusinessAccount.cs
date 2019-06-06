using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CharceApp.Models
{
    public class BusinessAccount
    {
        public int ID { get; set; }
        public int PersonalAccountID { get; set; }
        public string BusinessName { get; set; }
        public string BusinessType { get; set; }
        public string Location { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Website { get; set; }
    }
}