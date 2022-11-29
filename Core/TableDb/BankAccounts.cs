using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Core.TableDb
{
    public class BankAccounts
    {
        [Key]
        public int ID { get; set; }
        public string bankNameAr { get; set; }
        public string bankNameEn { get; set; }
        public string bankAccountNumber { get; set; }

        public string OwnerNameAr { get; set; }

        public int FK_BranchID { get; set; } //branch

        public bool isActive { get; set; }

    }
}
