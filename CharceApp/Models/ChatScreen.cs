using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CharceApp.Models
{
    public class ChatScreen
    {
        public int ID { get; set; }
        public bool hasMessage { get; set; }
        public string AccountType { get; set; }
        public int AccountID { get; set; }
    }
}