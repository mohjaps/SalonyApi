using System;
using System.Collections.Generic;

namespace Salony.ViewModels.Workers
{
    public class WorkerEvaOuptput
    {
        public int ID { get; set; }
        public String nameAr { get; set; }
        public String nameEn { get; set; }
        public String Image { get; set; }
        public String Description { get; set; }
        public double Points { get; set; }
        public List<SubServicesOutput> Services { get; set; }

    }
    public class SubServicesOutput
    {
        public int ID { get; set; }
        public string nameAr { get; set; }
        public string nameEn { get; set; }
        public double duration { get; set; }
        public double price { get; set; }
        public bool isActive { get; set; }
        public string Image { get; set; }
        public string DescriptionAr { get; set; }
        public string DescriptionEn { get; set; }
    }
}
