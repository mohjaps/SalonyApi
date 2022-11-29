using System.Collections.Generic;

namespace Salony.ViewModels
{
    public class CartVM
    {
        public int boutique_id { get; set; }
        public string img { get; set; }
        public int salonType { get; set; }
        public string name { get; set; }
        public string providerName { get; set; }
        public string address { get; set; }
        public string lat { get; set; }
        public string lng { get; set; }
        public int initial_distance { get; set; }
        public double initial_price { get; set; }
        public int kilo_price { get; set; }
        public int rate { get; set; }
        public double price { get; set; }
        public bool atHome { get; set; }
        public string note { get; set; }
        public IEnumerable<ServiceVM> services { get; set; }

    }
}
