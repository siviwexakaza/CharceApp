using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CharceApp.Models
{
    public class OptimaziedProductPic
    {
        public int ID { get; set; }
        public int ProductID { get; set; }
        public string ImageURL { get; set; }
        public int BusinessID { get; set; }
    }
}