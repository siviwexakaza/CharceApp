using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CharceApp.Models
{
    public class File_Product
    {
        public int File_ProductId { get; set; }
        public string FileName { get; set; }
        public int BusinessId { get; set; }
        public string ContentType { get; set; }
        public byte[] Content { get; set; }
        public FileType_Product FileType_Product { get; set; }
        public virtual ProfilePic_Product profilepic_product { get; set; }
        public int ProfilePic_ProductID { get; set; }
    }
}