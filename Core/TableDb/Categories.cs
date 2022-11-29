using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Core.TableDb
{
    public class Categories
    {
        [Key]
        public int ID { get; set; }
        public string nameAr { get; set; }
        public string nameEn { get; set; }
        public string img { get; set; }
        public bool isActive { get; set; }
        public int FK_BranchID { get; set; } //branch





        [InverseProperty(nameof(TableDb.MainServices.FK_Category))]
        public virtual ICollection<MainServices> MainServices { get; set; } = new HashSet<MainServices>();

        [InverseProperty(nameof(TableDb.ProviderAditionalData.FK_Category))]
        public virtual ICollection<ProviderAditionalData> ProviderAditionalData { get; set; } = new HashSet<ProviderAditionalData>();
    }
}
