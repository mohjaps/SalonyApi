using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Core.TableDb
{
    public class Sliders
    {
        [Key]
        public int ID { get; set; }
        public string img { get; set; }
        public bool isActive { get; set; }
        public int type { get; set; } // SliderType

        public int ProviderId { get; set; } = 0;

        public int FK_BranchID { get; set; } //branch


    }
}
