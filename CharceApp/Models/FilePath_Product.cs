using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CharceApp.Models
{
    public class FilePath_Product
    {
        public int FilePath_ProductId { get; set; }
        public string FileName { get; set; }
        public FileType_Product FileType_Business { get; set; }
        public int BusinessId { get; set; }
        public virtual ProfilePic_Product profilepic_product { get; set; }
        public int ProfilePic_ProductID { get; set; }
    }
}