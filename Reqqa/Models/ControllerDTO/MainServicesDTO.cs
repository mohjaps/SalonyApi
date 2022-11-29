using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Salony.Models.ControllerDTO
{
    public class MainServicesDTO
    {
        public int ID { get; set; }
        [Required(ErrorMessage = "من فضلك ادخل اسم الخدمة بالعربية")]
        public string nameAr { get; set; }
        [Required(ErrorMessage = "من فضلك ادخل اسم الخدمة بالانجليزية")]
        public string nameEn { get; set; }
      
        public bool isActive { get; set; }
        public int FK_BranchID { get; set; } //branch
        public int? FK_CategoryID { get; set; } //branch

        
    }
}
