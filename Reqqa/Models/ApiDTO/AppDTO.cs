using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Salony.Models.ApiDTO.App
{
    public class AboutAppDTO
    {
        [Required]
        public int branch_id { get; set; }
        public List<int> listint { get; set; } = new List<int>();
        /// <summary>
        ///ar or en
        /// </summary>
        /// <example>ar</example>
        public string lang { get; set; } = "ar";
    }
    public class ContactUsDTO
    {
        [Required]
        public string name { get; set; }
        public string email { get; set; }
        public string phone { get; set; }
        public string msg { get; set; }
        public int branch_id { get; set; }
        /// <summary>
        ///ar or en
        /// </summary>
        /// <example>ar</example>
        public string lang { get; set; } = "ar";
    }

  
}
