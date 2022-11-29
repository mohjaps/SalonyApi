using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Core.TableDb
{
    public class Orders
    {
        [Key]
        public int ID { get; set; }
        public DateTime date { get; set; }
        public int status { get; set; } // OrderStates
        public int typePay { get; set; } // TypePay
        public bool returnMoney { get; set; }
        public string copon { get; set; }
        public double discountPercentage { get; set; }
        public double priceBeforeDisc { get; set; }
        // قيمة الضريبة بعد الخصم
        public double valueOfDiscount { get; set; }
        public double valueOfTaxEleklil { get; set; }
        public double price { get; set; }
        
        public bool paid { get; set; }
        public bool payOut { get; set; }
        public int rate { get; set; }
        public string rateComment { get; set; }
        public string commentNote { get; set; }//Lady
        public string refusedReason { get; set; }
        public string pdf { get; set; }
        public double shippingPrice { get; set; } = 0;
        public DateTime orderDate { get; set; }
        public string address { get; set; } = string.Empty;
        public string lat { get; set; } = string.Empty;
        public string lng { get; set; } = string.Empty;

        public bool Applicationpercentagepaid { get; set; } = false;
        public double Applicationpercentage { get; set; } = 0;
        public double Adminpercentage { get; set; } = 0;
        public double Providerpercentage { get; set; } = 0;
        public bool ApplicationProviderpercentagepaid { get; set; } = false;
        public string ApplicationpercentageImg { get; set; }
        public bool IsDeleted { get; set; }


        public double AppCommission { get; set; } = 0;
        public double Deposit { get; set; } = 0;

        public string PaymentId { get; set; } // for hyperpay payment


        public string FK_UserID { get; set; }
        public int FK_ProviderID { get; set; }

        [ForeignKey(nameof(FK_UserID))]
        [InverseProperty(nameof(TableDb.ApplicationDbUser.clientOrders))]
        public virtual ApplicationDbUser FK_User { get; set; }

        [Required]
        [ForeignKey(nameof(FK_ProviderID))]
        [InverseProperty(nameof(TableDb.ProviderAditionalData.Orders))]
        public virtual ProviderAditionalData FK_Provider { get; set; }

        [InverseProperty(nameof(TableDb.OrderServices.FK_Order))]
        public virtual ICollection<OrderServices> OrderServices { get; set; } = new HashSet<OrderServices>();

        [InverseProperty(nameof(TableDb.Notifications.FK_Order))]
        public virtual ICollection<Notifications> Notifications { get; set; } = new HashSet<Notifications>();

        [InverseProperty(nameof(TableDb.Messages.FK_Order))]
        public virtual ICollection<Messages> Messages { get; set; } = new HashSet<Messages>();


    }
}
