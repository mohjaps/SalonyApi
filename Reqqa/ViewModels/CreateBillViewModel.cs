using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Salony.ViewModels
{
    public class CreateBillViewModel
    {
        public int id { get; set; }
        public string providerName { get; set; }
        public string date { get; set; }
        public string time { get; set; }
        public string commercialRegister { get; set; }
        public double price { get; set; }
        public double totalPrice { get; set; }
        public double priceBeforeDisc { get; set; }
        public double tax { get; set; }
        public double appPercent { get; set; }
        public double totalCut { get; set; }
        public int typePay { get; set; }
        public DateTime dateTime { get; set; }

        public List<CreateBillServiceViewModel> services { get; set; }
    }

public class CreateBillServiceViewModel
{
        public string name { get; set; }
        public double price { get; set; }
        public string date { get; set; }
        public string time { get; set; }
        public string address { get; set; }

    }
}
