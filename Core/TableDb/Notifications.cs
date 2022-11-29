using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Core.TableDb
{
    public class Notifications
    {
        [Key]
        public int ID { get; set; }
        public string msgAr { get; set; }
        public string msgEn { get; set; }
        public bool show { get; set; }
        public DateTime date { get; set; }
        public string FK_UserID { get; set; }
        public int? FK_OrderID { get; set; }

        [Required]
        [ForeignKey(nameof(FK_OrderID))]
        [InverseProperty(nameof(TableDb.Orders.Notifications))]
        public virtual Orders FK_Order { get; set; }

        [ForeignKey(nameof(FK_UserID))]
        [InverseProperty(nameof(TableDb.ApplicationDbUser.Notifications))]
        public virtual ApplicationDbUser FK_User { get; set; }


    }
}
