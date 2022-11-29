using System;
using System.ComponentModel.DataAnnotations;

namespace Salony.ViewModels.Workers
{
    public class NewEvaluation
    {
        public String Comment { get; set; }
        [Required, Range(0, 5)]
        public int Points { get; set; }
        [Required]
        public String UserID { get; set; }
        [Required]
        public String SenderID { get; set; }
    }
}
