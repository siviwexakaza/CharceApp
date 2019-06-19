using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CharceApp.Models
{
    public class Conversation
    {
        public int ID { get; set; }
        public int FirstPersonID { get; set; }
        public string FirstPersonDispName { get; set; }
        public int SecondPersonID { get; set; }
        public string SecondPersonDispName { get; set; }
        public DateTime Date { get; set; }
        public string LastMessage { get; set; }
        public int LastSenderID { get; set; }
        public bool Seen { get; set; }
    }
}