namespace Salony.ViewModels
{
    public class ServiceVM
    {
        public int id { get; set; }
        public string main_service { get; set; }
        public string name { get; set; }
        public string date { get; set; }
        public string time { get; set; }
        public string address { get; set; }
        public double servicePrice { get; set; }
        public double price { get; set; }
        public double deliveryPrice { get; set; }
        public double priceAtHome { get; set; }
        public double taxOfHome { get; set; }
        public double addedValuePrice { get; set; }
        public bool at_boutique { get; set; }
        public int? EmployeeID { get; set; }
        public string employeeName { get; set; }
        public string employeeImg { get; set; }
        public string note { get; set; }
        public string main_service_name { get; set; }
        public double duration { get; set; }
        public string lat { get; set; }
        public string lng { get; set; }
        public double distanceDetweenThem { get; set; }
        public double priceOfDelivery { get; set; }
    }
}
