using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Salony.ViewModels
{
    //public class UserInfoViewModel
    //{
    //    public string id { get; set; }
    //    public string user_name { get; set; }
    //    //public string email { get; set; }
    //    public string img { get; set; }
    //    public string phone { get; set; }
    //    public string lang { get; set; }
    //    public bool active { get; set; }
    //    public bool close_notify { get; set; }
    //    public int type { get; set; }

    //}

    public class UserInfoViewModel
    {
        public string id { get; set; }
        public string user_name { get; set; }
        public string email { get; set; } = null;
        public string img { get; set; }
        public string phone { get; set; }
        public string lang { get; set; }
        public bool active { get; set; }
        public bool close_notify { get; set; }
        public int type { get; set; }


        public string boutique_name_ar { get; set; } = null;
        public string boutique_name_en { get; set; } = null;
        public string commercial_register_number { get; set; } = null;
        public int city { get; set; } = 0;
        public int district { get; set; } = 0;
        public string address { get; set; } = null;
        public string lat { get; set; } = null;
        public string lng { get; set; } = null;
        public string time_from { get; set; } = null;
        public string time_to { get; set; } = null;
        public List<HelperDaysViewModel> days { get; set; } = null;
        public int salonType { get; set; } = 0;
        public string description_ar { get; set; } = null;
        public string description_en { get; set; } = null;
        public string iDPhoto { get; set; } = null;
        public string certificatePhoto { get; set; } = null;
        public string ibanNumber { get; set; } = null;
        public int categoryId { get; set; } = 0;
        public string categoryName { get; set; } = null;
        public string bankName { get; set; } = "";
        public string idNumber { get; set; } = "";
        public double stableWallet { get; set; } = 0;
        public int rate { get; set; } = 0;

        public string invitationCode { get; set; }

        public bool isAvailable { get; set; }


    }
}
