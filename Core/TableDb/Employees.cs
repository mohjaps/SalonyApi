using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Core.TableDb
{
    public class Employees
    {
        [Key]
        public int ID { get; set; }
        public string NameAr { get; set; }
        public string NameEn { get; set; }
        public string Img { get; set; }

        public int FK_ProviderAdditionalDataID { get; set; }

        [ForeignKey(nameof(FK_ProviderAdditionalDataID))]
        [InverseProperty(nameof(TableDb.ProviderAditionalData.Employees))]
        public virtual ProviderAditionalData FK_ProviderAdditionalData { get; set; }

        public int FK_SubServiceID { get; set; }

        [ForeignKey(nameof(FK_SubServiceID))]
        [InverseProperty(nameof(TableDb.SubServices.Employees))]
        public virtual SubServices FK_SubService { get; set; }

        [InverseProperty(nameof(TableDb.Carts.Fk_Employee))]
        public virtual ICollection<Carts> Carts { get; set; } = new HashSet<Carts>();

    }
}
