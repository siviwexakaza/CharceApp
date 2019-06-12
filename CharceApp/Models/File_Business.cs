using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CharceApp.Models
{
    public class File_Business
    {
        public int File_BusinessId { get; set; }
        public string FileName { get; set; }
        public int BusinessId { get; set; }
        public string ContentType { get; set; }
        public byte[] Content { get; set; }
        public FileType_Business FileType_Business { get; set; }
        public virtual ProfilePic_Business profilepic_business { get; set; }
        public int ProfilePic_BusinessID { get; set; }
    }
}