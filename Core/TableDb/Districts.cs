using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Core.TableDb
{
    public class Districts
    {
        [Key]
        public int ID { get; set; }
        public string nameAr { get; set; }
        public string nameEn { get; set; }
        public bool isActive { get; set; }


        public int FK_CityID { get; set; }


        [ForeignKey(nameof(FK_CityID))]
        [InverseProperty(nameof(Cities.Districts))]
        public virtual Cities FK_City { get; set; }


        
        [InverseProperty(nameof(TableDb.ProviderAditionalData.FK_District))]
        public virtual ICollection<ProviderAditionalData> ProviderAditionalData { get; set; } = new HashSet<ProviderAditionalData>();

    }
}
