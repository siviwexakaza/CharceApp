using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CharceApp.Models
{
    public class ProfilePic_Product
    {
        public int ID { get; set; }
        public int BusinessId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Item_Type { get; set; }
        public double Price { get; set; }
        public double Tax { get; set; }
        public virtual ICollection<File_Product> Files_Product { get; set; }
        public virtual ICollection<FilePath_Business> FilePaths_Product { get; set; }
    }
}