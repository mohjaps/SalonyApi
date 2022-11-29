using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Core.TableDb
{
    public class MainServices
    {
        [Key]
        public int ID { get; set; }
        public string nameAr { get; set; }
        public string nameEn { get; set; }
        public bool isActive { get; set; }

        public int? FK_CategoryID { get; set; }
        public int FK_BranchID { get; set; } //branch

        [ForeignKey(nameof(FK_CategoryID))]
        [InverseProperty(nameof(TableDb.Categories.MainServices))]
        public virtual Categories FK_Category { get; set; }

        [InverseProperty(nameof(TableDb.SubServices.FK_MainService))]
        public virtual ICollection<SubServices> SubServices { get; set; } = new HashSet<SubServices>();
    }
}
