using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CharceApp.Models
{
    public class FilePath
    {
        public int FilePathId { get; set; }
        public string FileName { get; set; }
        public FileType FileType { get; set; }
        public int UserId { get; set; }
        public virtual ProfilePic profilepic { get; set; }
        public int ProfilePicID { get; set; }
       
    }
}