using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CharceApp.Models
{
    public class OptimizedBusinessPic
    {
        public int ID { get; set; }
        public string ImageURL { get; set; }
        public int BusinessID { get; set; }
    }
}