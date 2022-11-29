using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Salony.Models.ApiDTO.Provider
{
    public class GetRegisterDataDTO
    {
        [Required]
        public int branch_id { get; set; } = 1;
        /// <summary>
        ///ar or en
        /// </summary>
        /// <example>ar</example>
        public string lang { get; set; } = "ar";
    }

    public class ListProviderMainServicesDTO
    {
        /// <summary>
        ///ar or en
        /// </summary>
        /// <example>ar</example>
        public string lang { get; set; } = "ar";
    }

    public class AddSubServiceDTO
    {
        public int main_service { get; set; }
        public string name_ar { get; set; }
        public string name_en { get; set; }
        public double duration { get; set; }
        public double price { get; set; }
        public IFormFile image { get; set; } = null;
        public string descriptionAr { get; set; } = null;
        public string descriptionEn { get; set; } = null;
        /// <summary>
        ///ar or en
        /// </summary>
        /// <example>ar</example>
        public string lang { get; set; } = "ar";
        public int WorkerID { get; set; }
    }

    public class EditSubServiceDTO
    {
        public int id { get; set; }
        public string name_ar { get; set; }
        public string name_en { get; set; }
        public double duration { get; set; }
        public double price { get; set; }
        public IFormFile image { get; set; } = null;
        public string descriptionAr { get; set; } = null;
        public string descriptionEn { get; set; } = null;
        /// <summary>
        ///ar or en
        /// </summary>
        /// <example>ar</example>
        public string lang { get; set; } = "ar";
    }

    public class DeleteSubServiceDTO
    {
        public int id { get; set; }
        /// <summary>
        ///ar or en
        /// </summary>
        /// <example>ar</example>
        public string lang { get; set; } = "ar";
    }

    public class GetCurrentBoutiqueDataDTO
    {
        /// <summary>
        ///ar or en
        /// </summary>
        /// <example>ar</example>
        public string lang { get; set; } = "ar";
    }

    public class GetCurrentBoutiqueAppPrecentDTO
    {
        /// <summary>
        ///ar or en
        /// </summary>
        /// <example>ar</example>
        public string lang { get; set; } = "ar";
    }

    public class ListProviderOffersDTO
    {
        /// <summary>
        ///ar or en
        /// </summary>
        /// <example>ar</example>
        public string lang { get; set; } = "ar";
    }

    public class AddProviderOfferDTO
    {
        public IFormFile img { get; set; } = null;
        /// <summary>
        ///ar or en
        /// </summary>
        /// <example>ar</example>
        public string lang { get; set; } = "ar";
    }

    public class DeleteProviderOfferDTO
    {
        public int id { get; set; }
        /// <summary>
        ///ar or en
        /// </summary>
        /// <example>ar</example>
        public string lang { get; set; } = "ar";
    }

    public class ListProviderOrdersDTO
    {
        /// <summary>
        ///   waiting = 0,
        ///   accepted = 1,
        ///   finished = 2,
        ///   refused = 3,
        ///   canceled = 4
        /// </summary>
        [Required]
        public int type { get; set; }
        /// <summary>
        ///ar or en
        /// </summary>
        /// <example>ar</example>
        public string lang { get; set; } = "ar";
    }

    public class GetOrderDetailsDTO
    {
        [Required]
        public int orderId { get; set; }
        /// <summary>
        ///ar or en
        /// </summary>
        /// <example>ar</example>
        public string lang { get; set; } = "ar";
    }

    public class ChangeOrderStatusDTO
    {
        public int id { get; set; }
        /// <summary>
        ///   waiting = 0,
        ///   accepted = 1,
        ///   finished = 2,
        ///   refused = 3,
        ///   canceled = 4
        /// </summary>
        [Required]
        public int type { get; set; }
        public string reason { get; set; }
        /// <summary>
        ///ar or en
        /// </summary>
        /// <example>ar</example>
        public string lang { get; set; } = "ar";
    }

    public class GetProviderDataDTO
    {
        /// <summary>
        ///ar or en
        /// </summary>
        /// <example>ar</example>
        public string lang { get; set; } = "ar";
    }

    public class UpdateProviderDataDTO
    {
        public IFormFile img { get; set; } = null;
        public string user_name { get; set; }
        public string boutique_name_ar { get; set; }
        public string boutique_name_en { get; set; }
        public string phone { get; set; }
        public string email { get; set; } = null;
        public string commercial_register_number { get; set; } = "";
        public int district { get; set; }
        public string address { get; set; }
        public string lat { get; set; }
        public string lng { get; set; }
        public DateTime time_from { get; set; }
        public DateTime time_to { get; set; }
        public string days { get; set; } = null;
        public int salonType { get; set; } = 0;
        public string description_ar { get; set; }
        public string description_en { get; set; }

        public IFormFile iDPhoto { get; set; } = null;
        public IFormFile certificatePhoto { get; set; } = null;

        public int category { get; set; } = 0;

        public List<IFormFile> imgs { get; set; } = new List<IFormFile>();
        public string deletedImgs { get; set; } = null;
        public string instagramProfile { get; set; } = null;

        public IFormFile identityImage { get; set; } = null;
        public string commercialRegister { get; set; } = null;
        public IFormFile commercialRegisterImage { get; set; } = null;
        public IFormFile healthCardImage { get; set; } = null;
        public string ibanNumber { get; set; } = null;
        public IFormFile ibanImage { get; set; } = null;

        /// <summary>
        /// رقم الهوية
        /// </summary>
        public string idNumber { get; set; }
        /// <summary>
        /// اسم البنك
        /// </summary>
        public string bankName { get; set; }



        /// <summary>
        ///ar or en
        /// </summary>
        /// <example>ar</example>
        public string lang { get; set; } = "ar";
    }

    public class CountProviderOrdersDTO
    {
        /// <summary>
        ///ar or en
        /// </summary>
        /// <example>ar</example>
        public string lang { get; set; } = "ar";

    }


    public class DeleteProviderNotifyDTO
    {
        public int id { get; set; }
        /// <summary>
        ///ar or en
        /// </summary>
        /// <example>ar</example>
        public string lang { get; set; } = "ar";
    }

}
