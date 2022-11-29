using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Salony.ViewModels.ChatViewModels
{
    public class SiteUsersChatViewModel
    {
        public int Id { get; set; }
        public int OrderNumber { get; set; }
        public string lastMsg { get; set; }
        public string UserId { get; set; }
        public string UserImg { get; set; }
        public string UserName { get; set; }
        public string Date { get; set; }
    }
}
