using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Salony.Models.ControllerDTO
{
    public class TermsAndConditionsDTO
    {
   
        [Required(ErrorMessage = "من فضلك ادخل هذا الحقل مطلوب")]

        public string ContentDataAR { get; set; }
        [Required(ErrorMessage = "من فضلك ادخل هذا الحقل مطلوب")]

        public string Media_Description2 { get; set; }
    }
}
