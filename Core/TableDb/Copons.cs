using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Core.TableDb
{
    public class Copons
    {
        [Key]
        public int ID { get; set; }
        public string code { get; set; }
        public double discPercentage { get; set; }
        public int FK_Branch { get; set; }
        public bool isActive { get; set; }

    }
}
