using System.Collections.Generic;

namespace Salony.ViewModels
{
    public class OrderVM
    {
        public int id { get; set; }
        public int boutique_id { get; set; }
        public string provider_id { get; set; }
        public string providerName { get; set; }
        public string boutique_name { get; set; }
        public string boutique_address { get; set; }
        public string boutique_img { get; set; }
        public int rate { get; set; }
        public int status { get; set; }
        public double price { get; set; }
        public double priceBeforDesc { get; set; }
        public double deliveryPrice { get; set; }
        public double priceWithDeliveryPrice { get; set; }
        public double valueOfDiscount { get; set; }
        public double discountPercentage { get; set; }
        public bool Paied { get; set; }
        public string pdf { get; set; }
        public double applicationPercent { get; set; }
        public string applicationPercentImg { get; set; }
        public string refused_reason { get; set; }
        public string payText { get; set; }
        public string address { get; set; }
        public string lat { get; set; }
        public string lng { get; set; }
        public string orderDate { get; set; }
        public string orderTime { get; set; }
        public string type { get; set; }
        public double shippingValue { get; set; }
        public bool isRate { get; set; }
        public double appCommission { get; set; }
        public double deposit { get; set; }
        public string paymentId { get; set; }
        public bool returnMoney { get; set; }
        public bool paidFromMobile { get; set; }
        public string status_name { get; set; }
        public int type_pay { get; set; }
        public string date { get; set; }
        public IEnumerable<ServiceVM> services { get; set; }

    }
}
