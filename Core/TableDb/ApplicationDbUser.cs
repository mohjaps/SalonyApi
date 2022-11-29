using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.TableDb
{
    public class ApplicationDbUser : IdentityUser
    {
        public string fullName { get; set; }
        public bool isActive { get; set; }
        public bool IsAvailable { get; set; }
        public bool isDeleted { get; set; }
        public string code { get; set; }
        public bool activeCode { get; set; }
        public string img { get; set; }
        public int typeUser { get; set; } //TypeUser
        public int FK_BranchID { get; set; } //branch
        public string showPassword { get; set; }
        public string lang { get; set; } = "ar";
        public DateTime registerDate { get; set; } = DateTime.Now;
        public DateTime sendCodeDate { get; set; } = DateTime.Now;
        public string addressName { get; set; }
        public string address { get; set; }
        public string lat { get; set; }
        public string lng { get; set; }
        public bool closeNotification { get; set; } = false;

        public string iDPhoto { get; set; } = null;
        public string certificatePhoto { get; set; } = null;
        public string ibanNumber { get; set; } = null;
        public double wallet { get; set; } = 0;
        public double stableWallet { get; set; } = 0;
        public string TempPaymentId { get; set; }

        public int PointsBalance { get; set; }
        /// <summary>
        /// user wallet for Payments and SaveOrder
        /// </summary>
        public double userWallet { get; set; } = 0;

        public string invitationCode { get; set; }


        [InverseProperty(nameof(TableDb.ProviderAditionalData.FK_User))]
        public virtual ProviderAditionalData ProviderAditionalData { get; set; }

        [InverseProperty(nameof(TableDb.Carts.FK_User))]
        public virtual ICollection<Carts> Carts { get; set; } = new HashSet<Carts>();

        [InverseProperty(nameof(TableDb.Orders.FK_User))]
        public virtual ICollection<Orders> clientOrders { get; set; } = new HashSet<Orders>();

        //[InverseProperty(nameof(TableDb.Orders.FK_Provider))]
        //public virtual ICollection<Orders> providerOrders { get; set; } = new HashSet<Orders>();

        [InverseProperty(nameof(TableDb.DeviceIds.FK_User))]
        public virtual ICollection<DeviceIds> DeviceIds { get; set; } = new HashSet<DeviceIds>();

        [InverseProperty(nameof(TableDb.Notifications.FK_User))]
        public virtual ICollection<Notifications> Notifications { get; set; } = new HashSet<Notifications>();

        [InverseProperty(nameof(TableDb.Messages.Sender))]
        public virtual ICollection<Messages> sender_Messages { get; set; } = new HashSet<Messages>();
        [InverseProperty(nameof(TableDb.Messages.Receiver))]
        public virtual ICollection<Messages> receiver_Messages { get; set; } = new HashSet<Messages>();
        [InverseProperty(nameof(TableDb.HistoryNotify.User))]
        public virtual ICollection<HistoryNotify> HistoryNotify { get; set; }

        [InverseProperty(nameof(TableDb.FinancialAccount.FKProvider))]
        public virtual ICollection<FinancialAccount> FinancialAccounts { get; set; }
        public virtual ICollection<Worker> Workers { get; set; }
        public virtual ICollection<WorkerEvaluation> WorkerEvaluations { get; set; }
    }
}
