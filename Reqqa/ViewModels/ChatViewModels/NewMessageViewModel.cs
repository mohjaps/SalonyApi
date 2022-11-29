using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Salony.ViewModels.ChatViewModels
{
    public class NewMessageViewModel
    {
        public string SenderId { get; set; }
        public string ReceiverId { get; set; }
        public string Text { get; set; }
        public int OrderId { get; set; }
        //public int OfferId { get; set; }
        public int Type { get; set; }
        public int Duration { get; set; }
    }
}
