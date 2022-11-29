using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Salony.Models.ChatDTO
{
    public class ConversationsDto
    {
       
        public string UserName { get; set; }
        public string UserId { get; set; }
        public string Img { get; set; }
        public string DateTime { get; set; }
        public int OrderNumber { get; set; }
    }
}
