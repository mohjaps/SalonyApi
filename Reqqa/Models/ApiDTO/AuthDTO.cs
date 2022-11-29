using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Salony.Models.ApiDTO
{
    public class RegisterClientDTO
    {
        [Required(ErrorMessage = "من فضلك ادخل اسم المستخدم")]
        public string user_name { get; set; }

        [Required(ErrorMessage = "من فضلك ادخل رقم الجوال")]
        public string phone { get; set; }

        [StringLength(500, MinimumLength = 6)]
        [Required(ErrorMessage = "من فضلك ادخل كلمة المرور")]
        public string password { get; set; }

        public string address { get; set; }
        public string email { get; set; } = null;
        public string lat { get; set; }
        public string lng { get; set; }

        public string invitationCode { get; set; }

        [Required]
        public int branch_id { get; set; }

        /// <summary>
        ///ar or en
        /// </summary>
        /// <example>ar</example>
        public string lang { get; set; } = "ar";

    }

    public class RegisterProviderDTO
    {
        [Required(ErrorMessage = "من فضلك ادخل صورة المستخدم")]
        public List<IFormFile> imgs { get; set; }

        [Required(ErrorMessage = "من فضلك ادخل اسم المستخدم")]
        public string user_name { get; set; }

        [Required(ErrorMessage = "من فضلك ادخل اسم المشغل عربى")]
        public string boutique_name_ar { get; set; }

        [Required(ErrorMessage = "من فضلك ادخل اسم المشغل انجليزى")]
        public string boutique_name_en { get; set; }

        [Required(ErrorMessage = "من فضلك ادخل رقم الجوال")]
        public string phone { get; set; }

        //[Required(ErrorMessage = "من فضلك ادخل الايميل")]
        //[EmailAddress(ErrorMessage = "من فضلك ادخل ايميل صحيح")]
        public string email { get; set; } = null;

        //[Required(ErrorMessage = "من فضلك ادخل السجل التجارى")]
        public string commercial_register_number { get; set; } = "";

        [Required(ErrorMessage = "من فضلك ادخل الحى")]
        public int district_id { get; set; }

        [Required(ErrorMessage = "من فضلك ادخل العنوان")]
        public string address { get; set; }

        [Required(ErrorMessage = "من فضلك ادخل العنوان")]
        public string lat { get; set; }

        [Required(ErrorMessage = "من فضلك ادخل العنوان")]
        public string lng { get; set; }

        [Required(ErrorMessage = "من فضلك ادخل بداية العمل")]
        public DateTime start_date { get; set; }

        [Required(ErrorMessage = "من فضلك ادخل نهاية العمل")]
        public DateTime end_date { get; set; }

        public string days { get; set; } = null;
        /// <summary>
        /// 1 => salon , 2 => home , 3 => both
        /// </summary>
        public int salonType { get; set; } = 3;

        /// <summary>
        /// 0 => both , 1 => men , 2 => women
        /// </summary>
        public int SalonUsersType { get; set; } = 0;

        [Required(ErrorMessage = "من فضلك ادخل الوصف عربى")]
        public string description_ar { get; set; }

        [Required(ErrorMessage = "من فضلك ادخل الوصف انجليزى")]
        public string description_en { get; set; }

        [Required(ErrorMessage = "من فضلك ادخل القسم")]
        public int category { get; set; }

        public IFormFile iDPhoto { get; set; } = null;
        public IFormFile certificatePhoto { get; set; } = null;
        public string socialMediaProfile { get; set; } = null;

        public IFormFile identityImage { get; set; } = null;
        public string commercialRegister { get; set; } = null;
        public IFormFile commercialRegisterImage { get; set; } = null;
        public IFormFile healthCardImage { get; set; } = null;
        public string ibanNumber { get; set; } = null;
        public IFormFile ibanImage { get; set; } = null;

        /// <summary>
        /// رقم الهوية
        /// </summary>
        public string idNumber { get; set; }
        
        /// <summary>
        /// اسم البنك
        /// </summary>
        public string bankName { get; set; }



        [StringLength(500, MinimumLength = 6)]
        [Required(ErrorMessage = "من فضلك ادخل كلمة المرور")]
        public string password { get; set; }


        public string bankAccount { get; set; } = null;

        [Required]
        public int branch_id { get; set; }

        /// <summary>
        ///ar or en
        /// </summary>
        /// <example>ar</example>
        public string lang { get; set; } = "ar";

    }

    public class ConfirmCodeRegisterDTO
    {

        [Required(ErrorMessage = "من فضلك ادخل كود التحقق")]
        public string code { get; set; }
        [Required]
        public string user_id { get; set; }

        /// <summary>
        ///ar or en
        /// </summary>
        /// <example>ar</example>
        public string lang { get; set; } = "ar";
    }

    public class LoginDTO
    {
        [Required(ErrorMessage = "من فضلك ادخل رقم الجوال")]
        public string phone { get; set; }
        [Required(ErrorMessage = "من فضلك ادخل كلمة المرور")]
        public string password { get; set; }
        [Required]
        public string device_id { get; set; }

        public int branch_id { get; set; } = 1;
        /// <summary>
        /// client = 1, provider = 2
        /// </summary>
        public int type_user { get; set; } = 0; // client = 1, provider = 2

        /// <summary>
        ///ar or en
        /// </summary>
        /// <example>ar</example>
        public string lang { get; set; } = "ar";
        public string deviceType { get; set; } = "android";
    }

    public class UpdateDataUserDTO
    {

        //[Required(ErrorMessage = "من فضلك ادخل البريد الالكترونى")]
        //[EmailAddress(ErrorMessage = " من فضلك ادخل البريد الالكترونى بشكل صحيح")]
        //public string email { get; set; }

        [Required(ErrorMessage = "من فضلك ادخل الاسم")]
        public string user_name { get; set; }

        [Required(ErrorMessage = "من فضلك ادخل رقم الجوال")]
        public string phone { get; set; }
        public string email { get; set; } = null;
        public IFormFile img { get; set; }

        /// <summary>
        ///ar or en
        /// </summary>
        /// <example>ar</example>
        public string lang { get; set; } = "ar";
        public string idNumber { get; set; }
        public string ibanNumber { get; set; }
        public string bankName { get; set; }

    }

    public class ChangePasswordDTO
    {

        [Required(ErrorMessage = "من فضلك ادخل كلمة المرور القديمة")]
        public string old_password { get; set; }

        [Required(ErrorMessage = "من فضلك ادخل كلمة المرور الجديدة")]
        public string new_password { get; set; }

        /// <summary>
        ///ar or en
        /// </summary>
        /// <example>ar</example>
        public string lang { get; set; } = "ar";
    }

    public class ForgetPasswordDTO
    {
        [Required(ErrorMessage = "من فضلك ادخل رقم الجوال")]
        public string phone { get; set; }

        public int branch_id { get; set; } = 1;

        /// <summary>
        /// client = 1, provider = 2
        /// </summary>
        public int type_user { get; set; } = 0; // client = 1, provider = 2

        /// <summary>
        ///ar or en
        /// </summary>
        /// <example>ar</example>
        public string lang { get; set; } = "ar";
    }

    public class ResendCodeDTO
    {
        public string user_id { get; set; }

        /// <summary>
        ///ar or en
        /// </summary>
        /// <example>ar</example>
        public string lang { get; set; } = "ar";
    }

    public class ChangePasswordByCodeDTO
    {
        [Required]
        public string user_id { get; set; }

        [Required(ErrorMessage = "من فضلك ادخل كود التحقق")]
        public string code { get; set; }

        [Required(ErrorMessage = "من فضلك ادخل كلمة المرور الجديدة")]
        public string new_password { get; set; }

        /// <summary>
        ///ar or en
        /// </summary>
        /// <example>ar</example>
        public string lang { get; set; } = "ar";
    }

    public class LogoutDTO
    {
        public string device_id { get; set; } = "";

        /// <summary>
        ///ar or en
        /// </summary>
        /// <example>ar</example>
        public string lang { get; set; } = "ar";

    }

    public class ChangeLanguageDTO
    {
        public string language { get; set; } = null;

        /// <summary>
        ///ar or en
        /// </summary>
        /// <example>ar</example>
        public string lang { get; set; } = "ar";

    }



}
