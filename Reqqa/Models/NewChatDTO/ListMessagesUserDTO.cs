using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Salony.Models.NewChatDTO
{
    public class ListMessagesUserDTO
    {
        public int OrderId { get; set; }
        public int pageNumber { get; set; } = 50;
    }
}
