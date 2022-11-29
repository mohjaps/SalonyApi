using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Core.TableDb
{
    public class HistoryNotify
    {
        [Key]
        public int Id { get; set; }
        public string Text { get; set; }
        public DateTime Date { get; set; }
        public string FKUser { get; set; }
        [Required]
        [ForeignKey(nameof(FKUser))]
        public virtual ApplicationDbUser User { get; set; }
    }
}
