using Salony.Models.ChatDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Salony.ViewModels.ChatViewModels
{
    public class PushMessegeViewModel : MessageTwoUsersDto
    {
        public int start { get; set; }
        public int position { get; set; }
        public int duration { get; set; }
        public bool play { get; set; } = false;
    }
}
