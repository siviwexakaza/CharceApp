using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CharceApp.Models;

namespace CharceApp.Models
{
    public class ProfilePic
    {
        public int ID { get; set; }
        public int UserId { get; set; }
        public virtual ICollection<File> Files { get; set; }
        public virtual ICollection<FilePath> FilePaths { get; set; }
    }
}