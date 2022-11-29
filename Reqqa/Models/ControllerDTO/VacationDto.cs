using System;

namespace Salony.Models.ControllerDTO
{
    public class VacationDto
    {
        public string userId { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
    }
}
