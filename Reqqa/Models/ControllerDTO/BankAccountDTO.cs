using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Salony.Models.ControllerDTO
{
    public class BankAccountDTO
    {
        public int ID { get; set; }
        [Required(ErrorMessage = "من فضلك ادخل اسم البنك بالعربية")]
        public string bankNameAr { get; set; }
        [Required(ErrorMessage = "من فضلك ادخل اسم البنك بالانجليزية")]
        public string bankNameEn { get; set; }
        [Required(ErrorMessage = "من فضلك ادخل رقم الحساب البنكى")]
        public string bankAccountNumber { get; set; } 
        [Required(ErrorMessage = "من فضلك ادخل رقم اسم صاحب الحساب")]
        public string OwnerName { get; set; }
        public bool isActive { get; set; }

    }
}
