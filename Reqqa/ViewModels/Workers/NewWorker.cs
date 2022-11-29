using Microsoft.AspNetCore.Http;
using System;
using System.ComponentModel.DataAnnotations;

namespace Salony.ViewModels.Workers
{
    public class NewWorker
    {
        [Required]
        public String nameAr { get; set; }
        [Required]
        public String nameEn { get; set; }
        public String? Description { get; set; }
        [Required]
        public IFormFile Image { get; set; }
        [Required]
        public String AttendanceDays { get; set; }
        [Required]
        public String SallonID { get; set; }
    }
}
