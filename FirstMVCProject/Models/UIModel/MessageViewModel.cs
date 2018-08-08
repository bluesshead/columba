using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FirstMVCProject.Models.UIModel
{
    public class MessageViewModel
    {
        public username User { get; set; }
        public List<chat> MessageList { get; set; }
        public chat Message { get; set; }
    }
}