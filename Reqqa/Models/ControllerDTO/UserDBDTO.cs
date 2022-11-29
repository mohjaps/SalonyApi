using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Salony.Models.ControllerDTO
{
    public class UserDBDTO
    {
        public string id { get; set; }
        public string Email { get; set; }
        public string registerDate { get; set; }
        public DateTime registerDateToSort { get; set; }
        public string PhoneNumber { get; set; }
        public string fullName { get; set; }
        public string img { get; set; }
        public string lang { get; set; }
        public string address { get; set; }
        public string code { get; set; }
        public int cartCount { get; set; }
        public double userWallet { get; set; }
        public bool isActive { get; set; }
        public bool isDeleted { get; set; }
        public string invitationCodeBallance { get; set; }

    }
}
