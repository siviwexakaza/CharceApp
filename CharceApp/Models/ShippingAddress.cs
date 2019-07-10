using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CharceApp.Models
{
    public class ShippingAddress
    {
        public int ID { get; set; }
        public int PersonalAccID { get; set; }
        public string RecipientName { get; set; }
        public string RecipientNumber { get; set; }
        public string StreetAddress { get; set; }
        public string Building { get; set; }
        public string Suburb { get; set; }
        public string City { get; set; }
        public string Province { get; set; }
        public string PostalCode { get; set; }
    }
}