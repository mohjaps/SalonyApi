using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Core.TableDb
{
    public class FinancialAccount
    {
        [Key]
        public int Id { get; set; }
        public string FkProviderId { get; set; }
        public double PayOutPrice { get; set; }
        public bool IsPaid { get; set; }
        public DateTime Date { get; set; }


        [ForeignKey(nameof(FkProviderId))]
        [InverseProperty(nameof(ApplicationDbUser.FinancialAccounts))]
        public virtual ApplicationDbUser FKProvider { get; set; }
    }
}
