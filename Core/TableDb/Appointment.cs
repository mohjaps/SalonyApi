using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Core.TableDb
{
    public class Appointment
    {
        [Key]
        public int ID { get; set; }
        [Required, ForeignKey(nameof(Sallon))]
        public String SallonID { get; set; }
        public ApplicationDbUser Sallon { get; set; }

        [Required, ForeignKey(nameof(User))]
        public String UserID { get; set; }
        public ApplicationDbUser User { get; set; }

        public DateTime Date { get; set; }
    }
}
