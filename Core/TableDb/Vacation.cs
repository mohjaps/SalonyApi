using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Core.TableDb
{
    public class Vacation
    {
        [Key]
        public int Id { get; set; }
        [Required, ForeignKey(nameof(User))]
        public String UserID { get; set; }
        public ApplicationDbUser User { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
    }
}
