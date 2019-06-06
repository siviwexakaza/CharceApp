using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CharceApp.Models
{
    public class PersonalAccount
    {
        public int ID { get; set; }
        public string PhoneNumber { get; set; }
        public string Names { get; set; }
        public string Surname { get; set; }
        public string Province { get; set; }
        public string City { get; set; }
        public virtual ApplicationUser AppUser { get; set; }
        public string AppUserId { get; set; }
    }
}