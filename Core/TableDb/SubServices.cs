using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.TableDb
{
    public class SubServices
    {
        [Key]
        public int ID { get; set; }
        public string nameAr { get; set; }
        public string nameEn { get; set; }
        public double duration { get; set; }
        public double price { get; set; }
        public bool isActive { get; set; }
        public string Image { get; set; }
        public string DescriptionAr { get; set; }
        public string DescriptionEn { get; set; }
        public int FK_MainServiceID { get; set; }
        public int FK_ProviderAdditionalDataID { get; set; }

        [ForeignKey(nameof(FK_MainServiceID))]
        [InverseProperty(nameof(TableDb.MainServices.SubServices))]
        public virtual MainServices FK_MainService { get; set; }

        [ForeignKey(nameof(FK_ProviderAdditionalDataID))]
        [InverseProperty(nameof(TableDb.ProviderAditionalData.SubServices))]
        public virtual ProviderAditionalData FK_ProviderAdditionalData { get; set; }

        [InverseProperty(nameof(TableDb.Carts.FK_SubService))]
        public virtual ICollection<Carts> Carts { get; set; } = new HashSet<Carts>();

        [InverseProperty(nameof(TableDb.Employees.FK_SubService))]
        public virtual ICollection<Employees> Employees { get; set; } = new HashSet<Employees>();

        [ForeignKey(nameof(Worker))]
        public int WokerID { get; set; }
        public Worker Worker { get; set; }

    }
}
