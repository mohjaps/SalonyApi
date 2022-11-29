using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Salony.ViewModels.ChatViewModels
{
    public class SiteMessagesViewModel
    {
        public int id { get; set; }
        public int orderNumber { get; set; }
        public int type { get; set; }
        public string senderId { get; set; }
        public string receiverId { get; set; }
        public string senderImg { get; set; }
        public string receiverImg { get; set; }
        public string message { get; set; }
        public string date { get; set; }
        public bool sender { get; set; }
    }
}
