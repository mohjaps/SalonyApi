using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Salony.Models.ControllerDTO
{
    public class CoponsDTO
    {
        public int ID { get; set; }
        [Required(ErrorMessage = "من فضلك ادخل الكود")]
        public string code { get; set; }
        [Required(ErrorMessage = "من فضلك ادخل نسبة الخصم")]
        public double discPercentage { get; set; }
        public bool isActive { get; set; }

    }
}
