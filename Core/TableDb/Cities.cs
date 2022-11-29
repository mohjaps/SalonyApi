using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Core.TableDb
{
    public class Cities
    {
        [Key]
        public int ID { get; set; }
        public string nameAr { get; set; }
        public string nameEn { get; set; }
        public int FK_BranchID { get; set; } //branch
        public bool isActive { get; set; }

        [InverseProperty(nameof(TableDb.Districts.FK_City))]
        public virtual ICollection<Districts> Districts { get; set; } = new HashSet<Districts>();

    }
}
