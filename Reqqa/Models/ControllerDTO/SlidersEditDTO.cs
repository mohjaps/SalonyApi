using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Salony.Models.ControllerDTO
{
    public class SlidersEditDTO
    {
        [Key]
        public int ID { get; set; }
        // img not required in Edit
        public IFormFile img { get; set; } = null;
        public bool isActive { get; set; }
        public int type { get; set; } // SliderType
        public int ProviderId { get; set; }
        public string DisplayImg { get; set; } // DisplayImg
        //[Required(AllowEmptyStrings = false, ErrorMessage = "من فضلك اختر مقدم الخدمة")]
        public string FK_BranchID { get; set; } //branch
    }
}
