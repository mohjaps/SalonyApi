using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Salony.Models.ControllerDTO
{
    public class SettingsDTO
    {
        [Key]
        public int ID { get; set; }
        [Required(ErrorMessage = "من فضلك ادخل هذا الحقل مطلوب")]

        public string keyMap { get; set; }

        public string aboutAppAr { get; set; }
        public string aboutAppEn { get; set; }
        public string condtionsAr { get; set; }
        public string condtionsEn { get; set; }
        public string paymentPolicyAr { get; set; }
        [Range(0,int.MaxValue)]
        public int ExpireTime { get; set; }
        public string paymentPolicyEn { get; set; }
        [Required(ErrorMessage = "من فضلك ادخل هذا الحقل مطلوب")]

        public string appleStoreUrl { get; set; }
        [Required(ErrorMessage = "من فضلك ادخل هذا الحقل مطلوب")]

        public string googlePlayUrl { get; set; }

        [Required(ErrorMessage = "من فضلك ادخل هذا الحقل مطلوب")]
        [MinLength(10,ErrorMessage = "رقم الهاتف اقل من 10 ارقام")]
        public string phone { get; set; }

        [Required(ErrorMessage = "من فضلك ادخل هذا الحقل مطلوب")]
        [MinLength(10, ErrorMessage = "رقم الهاتف اقل من 10 ارقام")]
        public string phone2 { get; set; }
        [Required(ErrorMessage = "من فضلك ادخل هذا الحقل مطلوب")]

        public string facebook { get; set; }
        [Required(ErrorMessage = "من فضلك ادخل هذا الحقل مطلوب")]

        public string twitter { get; set; }
        [Required(ErrorMessage = "من فضلك ادخل هذا الحقل مطلوب")]

        public string telegram { get; set; }
        [Required(ErrorMessage = "من فضلك ادخل هذا الحقل مطلوب")]

        public string instagram { get; set; }
        [Required(ErrorMessage = "من فضلك ادخل هذا الحقل مطلوب")]
        [MinLength(10, ErrorMessage = "رقم الواتس اب اقل من 10 ارقام")]
        public string whatsApp { get; set; }
        [Required(ErrorMessage = "من فضلك ادخل هذا الحقل مطلوب")]

        public string snapChat { get; set; }

        [Required(ErrorMessage = "من فضلك ادخل هذا الحقل مطلوب")]
        public double appPrecent { get; set; }
        
        [Required(ErrorMessage = "من فضلك ادخل هذا الحقل مطلوب")]
        public double appPrecentPercentage { get; set; }
        
        [Required(ErrorMessage = "من فضلك ادخل هذا الحقل مطلوب")]

        public string youtube { get; set; }

        public double invitationCodeBallance { get; set; }

        public string commercialRegister { get; set; }
        public string patText { get; set; }
        public double tax { get; set; }
        public double taxOfHome { get; set; } = 0;


        public string Screen1TitleAr { get; set; } = null;
        public string Screen1TitleEn { get; set; } = null;
        public string Screen2TitleAr { get; set; } = null;
        public string Screen2TitleEn { get; set; } = null;
        public string Screen3TitleAr { get; set; } = null;
        public string Screen3TitleEn { get; set; } = null;
        public string Screen1DescriptionAr { get; set; } = null;
        public string Screen1DescriptionEn { get; set; } = null;
        public string Screen2DescriptionAr { get; set; } = null;
        public string Screen2DescriptionEn { get; set; } = null;
        public string Screen3DescriptionAr { get; set; } = null;
        public string Screen3DescriptionEn { get; set; } = null;



        public int FK_BranchID { get; set; } //branch


    }
}
