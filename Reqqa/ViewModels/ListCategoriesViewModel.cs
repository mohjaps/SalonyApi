using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Salony.ViewModels
{
    public class ListCategoriesViewModel
    {
        public int id { get; set; }
        public string name { get; set; }
        public string img { get; set; }
        public List<ListCategoriesBoutiquesViewModel> boutiques { get; set; } = new List<ListCategoriesBoutiquesViewModel>();
    }
    public class ListCategoriesBoutiquesViewModel
    {
        public int id { get; set; }
        public string name { get; set; }
        public string img { get; set; }
        public string description { get; set; }
        public string address { get; set; }
        public int rate { get; set; }
        public string dayWorks { get; set; }
        public DateTime timeFrom { get; set; }
        public DateTime timeTo { get; set; }
        public bool opened { get; set; }

    }
}
