using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Salony.Models.ControllerDTO
{
    public class ProviderAditionalDataDTO
    {
        public string id { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string fullName { get; set; }
        public string img { get; set; }
        public string registerDate { get; set; }
        public DateTime registerDateToSort { get; set; }
        public string lang { get; set; }
        public string address { get; set; }
        public string code { get; set; }
        public string password { get; set; }
        public bool isActive { get; set; }
        public bool isDeleted { get; set; }
        public string UserIbanNumber { get; set; }


        // Provider Aditional Data
        public int ProviderID { get; set; }
        public string nameAr { get; set; }
        public string nameEn { get; set; }
        public string descriptionAr { get; set; }
        public int rate { get; set; }
        public DateTime timeForm { get; set; }
        public DateTime timeTo { get; set; }

        public string certificatePhoto { get; set; }
        public string bankAccount { get; set; }

        public double neededCommission { get; set; }
        public double neededWalletCommission { get; set; }
        public double wallet { get; set; }
        public double userWallet { get; set; }

        public int subService { get; set; }
        //---------------------
        public double allDdeposit { get; set; }
        public double allCommission { get; set; }
        //-------------------------------

        // Name Of Destrict And City 
        public string destrict { get; set; }
        public string city { get; set; }



        public string IdentityImage { get; set; }
        public string commercialRegister { get; set; }
        public string CommercialRegisterImage { get; set; }
        public string HealthCardImage { get; set; }
        public string IbanNumber { get; set; }
        public string IbanImage { get; set; }

    }
}
