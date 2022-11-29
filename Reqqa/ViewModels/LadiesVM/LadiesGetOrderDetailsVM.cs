using System.Collections.Generic;

namespace Salony.ViewModels.LadiesVM
{
    public class LadiesGetOrderDetailsVM
    {
        public int id { get; set; }
        public string client_id { get; set; }
        public string client_name { get; set; }
        public string client_phone { get; set; }
        public string client_img { get; set; }
        public int count_services { get; set; }
        public int type_pay_id { get; set; }
        public string type_pay { get; set; }
        public double price { get; set; }
        //public double priceAtHome { get; set; }
        //public double deliveryPrice { get; set; }
        public double priceBeforDesc { get; set; }
        public double applicationPercent { get; set; }
        public string pdf { get; set; }
        public string address { get; set; }
        public string lat { get; set; }
        public string lng { get; set; }
        public string orderDate { get; set; }
        public string orderTime { get; set; }
        public string type { get; set; }
        public double shippingValue { get; set; }
        public int status { get; set; }
        public int rate { get; set; }
        public string comment { get; set; }
        public List<string> comments { get; set; }
        public string commentNote { get; set; }
        public string refusedReason { get; set; }
        public List<orderServices> services { get; set; }
    }
}
