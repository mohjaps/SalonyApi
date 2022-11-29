using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Core.TableDb
{
    public class Offers
    {
        [Key]
        public int ID { get; set; }
        public string nameAr { get; set; }
        public string nameEn { get; set; }
        public string img { get; set; }
        public bool isActive { get; set; }

        public int FK_ProviderAdditionalDataID { get; set; }


        [ForeignKey(nameof(FK_ProviderAdditionalDataID))]
        [InverseProperty(nameof(TableDb.ProviderAditionalData.Offers))]
        public virtual ProviderAditionalData FK_ProviderAdditionalData { get; set; }

    }
}
