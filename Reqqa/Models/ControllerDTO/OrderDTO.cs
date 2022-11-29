using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Salony.Models.ControllerDTO
{
    public class OrderDTO
    {

        public int ID { get; set; }


        [Display(Name = "التاريخ")]
        [Required(ErrorMessage = "اختر التاريخ")]
        public DateTime date { get; set; }

        [Display(Name = "حالة الطلب")]
        [Required(ErrorMessage = "اختر حالة الطلب")]
        public int status { get; set; } // OrderStates
        
        [Display(Name = "طريقة الدفع")]
        [Required(ErrorMessage = "اختر طريقة الدفع")]
        public int typePay { get; set; } // TypePay

        public bool returnMoney { get; set; }

        [Display(Name = "الكوبون")]
        //[Required(ErrorMessage = "ادخل الكوبون")]
        public string copon { get; set; }

        [Display(Name = "نسبة الخصم")]
        public double discountPercentage { get; set; }
        
        [Display(Name = "السعر قبل الخصم")]
        public double priceBeforeDisc { get; set; }
        
        [Display(Name = "السعر")]
        [Required(ErrorMessage = "ادخل السعر")]
        public double price { get; set; }

        [Display(Name = "مدفوع")]
        public bool paid { get; set; }
        
        //[Display(Name = "التقييم")]
        public int rate { get; set; }
        
        [Display(Name = "سبب الرفض")]
        public string refusedReason { get; set; }


        public string FK_UserID { get; set; }
        public int FK_ProviderID { get; set; }
    }
}
