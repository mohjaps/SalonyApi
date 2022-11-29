using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Salony.Models.ControllerDTO
{
    public class DistrictsDTO
    {
        public int ID { get; set; }
        [Required(ErrorMessage ="من فضلك ادخل اسم الحى بالعربية")]
        public string nameAr { get; set; }
        [Required(ErrorMessage = "من فضلك ادخل اسم الحى بالإنجليزية")]
        public string nameEn { get; set; }
        public bool isActive { get; set; }
        public int FK_CityID { get; set; }
    }
}
