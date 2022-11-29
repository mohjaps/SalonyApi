using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Salony.Models.ControllerDTO
{
    public class CitiesDTO
    {
        public int ID { get; set; }
        [Required(ErrorMessage = "من فضلك ادخل اسم المدينة بالعربية")]
        public string nameAr { get; set; }
        [Required(ErrorMessage = "من فضلك ادخل اسم المدينة بالإنجليزية")]
        public string nameEn { get; set; }
        public int FK_BranchID { get; set; } //branch
        public bool isActive { get; set; }
    }
}
