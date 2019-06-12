using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CharceApp.Models
{
    public class ProfilePic_Business
    {
        public int ID { get; set; }
        public int BusinessId { get; set; }
        public virtual ICollection<File_Business> Files_Business { get; set; }
        public virtual ICollection<FilePath_Business> FilePaths_Business { get; set; }
    }
}