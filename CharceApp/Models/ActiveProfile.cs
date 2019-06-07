using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CharceApp.Models
{
    public class ActiveProfile
    {
        public int ID { get; set; }
        public virtual ApplicationUser AppUser { get; set; }
        public string ApplicationUserId { get; set; }

        public int ActiveProfileID { get; set; }
        public string AccountType { get; set; }
    }
}