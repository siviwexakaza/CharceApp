using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CharceApp.Models
{
    public class Message
    {
        public int ID { get; set; }
        public DateTime Date { get; set; }
        public int ConversationID { get; set; }
        public int SenderID { get; set; }
        public string SenderDispName { get; set; }
        public bool Seen { get; set; }
        public string Text { get; set; }
        public bool isOrder { get; set; }
        public int OrderID { get; set; }
    }
}