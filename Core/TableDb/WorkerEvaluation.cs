using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.TableDb
{
    public class WorkerEvaluation
    {
        [Key]
        public int ID { get; set; }
        public String Comment { get; set; }
        [Range(0, 5)]
        public int Points { get; set; }
        [Required, ForeignKey(nameof(Worker))]
        public int WorkerID { get; set; }
        public Worker Worker { get; set; }

        [Required, ForeignKey(nameof(User))]
        public String UserID { get; set; }
        public ApplicationDbUser User { get; set; }
    }
}
