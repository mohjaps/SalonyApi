using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.TableDb
{
    public class Worker
    {
        [Key]
        public int ID { get; set; }
        [Required]
        public String nameAr { get; set; }
        [Required]
        public String nameEn { get; set; }
        public String? Description { get; set; }
        [Required]
        public String Image { get; set; }
        [Required]
        public String AttendanceDays { get; set; }

        [Required, ForeignKey(nameof(Sallon))]
        public String SallonID { get; set; }
        public ApplicationDbUser Sallon { get; set; }
        public List<WorkerEvaluation> Evaluation { get; set; }
        public List<SubServices> SubServices { get; set; }

    }
}
