using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Core.TableDb
{
    public class Carts
    {
        [Key]
        public int ID { get; set; }
        public DateTime date { get; set; }
        public string address { get; set; }
        public string note { get; set; }
        public string lat { get; set; }
        public string lng { get; set; }


        public int FK_SubServiceID { get; set; }
        public string? FK_UserID { get; set; }
        public int? Fk_EmployeeID { get; set; }


        [ForeignKey(nameof(FK_SubServiceID))]
        [InverseProperty(nameof(TableDb.SubServices.Carts))]
        public virtual SubServices FK_SubService { get; set; }

        [ForeignKey(nameof(FK_UserID))]
        [InverseProperty(nameof(TableDb.ApplicationDbUser.Carts))]
        public virtual ApplicationDbUser FK_User { get; set; }

        [ForeignKey(nameof(Fk_EmployeeID))]
        [InverseProperty(nameof(TableDb.Employees.Carts))]
        public virtual Employees Fk_Employee { get; set; }

    }
}
