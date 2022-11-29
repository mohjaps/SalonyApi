using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Salony.Models.ControllerDTO
{
    public class CategoryEditDTO
    {
        public int ID { get; set; }
        [Required(ErrorMessage = "من فضلك ادخل اسم الصنف بالعربية")]
        public string nameAr { get; set; }
        [Required(ErrorMessage = "من فضلك ادخل اسم الصنف بالانجليزية")]
        public string nameEn { get; set; }
        public IFormFile img { get; set; }
        public string DisplayImg { get; set; } // DisplayImg

        public bool isActive { get; set; }
        public int FK_BranchID { get; set; } //branch
    }
}
