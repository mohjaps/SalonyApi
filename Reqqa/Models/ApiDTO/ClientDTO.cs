using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Salony.Models.ApiDTO.Client
{
    public class ListCategoriesDTO
    {
        public int branch_id { get; set; }
        /// <summary>
        ///ar or en
        /// </summary>
        /// <example>ar</example>
        public string lang { get; set; } = "ar";
    }

    public class ListBoutiquesDTO
    {
        public int category_id { get; set; }
        public int branch_id { get; set; }
        public string lat { get; set; }
        public string lng { get; set; }

        /// <summary>
        ///ar or en
        /// </summary>
        /// <example>ar</example>
        public string lang { get; set; } = "ar";
    }
    public class SearchDTO
    {
        public string text { get; set; }
        public int branch_id { get; set; }

        /// <summary>
        ///ar or en
        /// </summary>
        /// <example>ar</example>
        public string lang { get; set; } = "ar";
    }
    public class SearchByServiceDTO
    {
        public string text { get; set; }
        public int branch_id { get; set; }

        /// <summary>
        ///ar or en
        /// </summary>
        /// <example>ar</example>
        public string lang { get; set; } = "ar";
    }
    public class ListMainServicesDTO
    {
        public int branch_id { get; set; }
        public int category_id { get; set; } = 0;

        /// <summary>
        ///ar or en
        /// </summary>
        /// <example>ar</example>
        public string lang { get; set; } = "ar";
    }
    public class FilterBoutiquesDTO
    {
        //public double rate { get; set; } = 0;
        public int categoryID { get; set; } = 0;
        public bool top_rate { get; set; } = false;
        public string main_services { get; set; } = null;
        public double min_price { get; set; } = 0;
        public double max_price { get; set; } = 10000;
        public string lat { get; set; }
        public string lng { get; set; }
        public int branch_id { get; set; }

        /// <summary>
        ///ar or en
        /// </summary>
        /// <example>ar</example>
        public string lang { get; set; } = "ar";
    }
    public class GetBoutiqueDTO
    {
        public int boutique_id { get; set; }

        /// <summary>
        ///ar or en
        /// </summary>
        /// <example>ar</example>
        public string lang { get; set; } = "ar";
    }

    public class AddToCartDTO
    {
        [Required]
        public int service_id { get; set; }
        /// <summary>
        /// dd/MM/yyyy
        /// </summary>
        public string date { get; set; } = "01/01/2020";
        /// <summary>
        /// h:mm tt
        /// </summary>
        public string time { get; set; } = "1:01 AM";
        public bool home { get; set; } = false;
        public string address { get; set; } = null;
        public string lat { get; set; } = null;
        public string lng { get; set; } = null;
        public string notes { get; set; } = "";
        public int employeeId { get; set; } = 0;

        /// <summary>
        ///ar or en
        /// </summary>
        /// <example>ar</example>
        public string lang { get; set; } = "ar";
    }

    public class GetCartDTO
    {
        /// <summary>
        ///ar or en
        /// </summary>
        /// <example>ar</example>
        public string lang { get; set; } = "ar";
    }
    public class GetCartWithDetailsDTO
    {
        public int boutiqueId { get; set; }
        /// <summary>
        ///ar or en
        /// </summary>
        /// <example>ar</example>
        public string lang { get; set; } = "ar";
    }

    public class DeleteServiceFromCartDTO
    {
        public int service_id { get; set; }
        /// <summary>
        ///ar or en
        /// </summary>
        /// <example>ar</example>
        public string lang { get; set; } = "ar";
    }

    public class SaveOrderDTO
    {
        public int boutique_id { get; set; }
        /// <summary>
        /// 0 => cash , 1 => online
        /// </summary>
        public int type_pay { get; set; }
        public string code { get; set; } = null;
        /// <summary>
        /// dd/MM/yyyy
        /// </summary>
        public string date { get; set; } = "01/01/2020";
        /// <summary>
        /// HH:mm tt
        /// </summary>
        public string time { get; set; } = "01:01 AM";
        public bool home { get; set; } = false;
        public string address { get; set; } = null;
        public string lat { get; set; } = null;
        public string lng { get; set; } = null;

        public double shippingPrice { get; set; } = 0;
        /// <summary>
        ///ar or en
        /// </summary>
        /// <example>ar</example>    

        public string Notes { get; set; }
        public string lang { get; set; } = "ar";
    }

    public class ListOrdersDTO
    {
        /// <summary>
        ///ar or en
        /// </summary>
        /// <example>ar</example>
        public string lang { get; set; } = "ar";
    }
    public class ListAllOrdersDTO
    {
        /// <summary>
        ///ar or en
        /// </summary>
        /// <example>ar</example>
        public string lang { get; set; } = "ar";
    }

    public class PayOrderDTO
    {
        public int order_id { get; set; }
        public int paymentType { get; set; }
        public IFormFile img { get; set; }
        /// <summary>
        ///ar or en
        /// </summary>
        /// <example>ar</example>
        public string lang { get; set; } = "ar";
    }

    public class CancelOrderDTO
    {
        public int order_id { get; set; }
        public string refusedReason { get; set; } = null;
        /// <summary>
        ///ar or en
        /// </summary>
        /// <example>ar</example>
        public string lang { get; set; } = "ar";
    }

    public class RateOrderDTO
    {
   
        public int order_id { get; set; }
        public int rate { get; set; }
        public string comment { get; set; } = null;
        public string note { get; set; } = "";
        /// <summary>
        ///ar or en
        /// </summary>
        /// <example>ar</example>
        public string lang { get; set; } = "ar";
    }

    public class GetOrderDTO
    {
        public int order_id { get; set; }
        /// <summary>
        ///ar or en
        /// </summary>
        /// <example>ar</example>
        public string lang { get; set; } = "ar";
    }

    public class ListNotificationsDTO
    {
        /// <summary>
        ///ar or en
        /// </summary>
        /// <example>ar</example>
        public string lang { get; set; } = "ar";
    }   
    
    public class DeleteNotificationsDTO
    {
        /// <summary>
        ///ar or en
        /// </summary>
        /// <example>ar</example>
        public string lang { get; set; } = "ar";
    }   
    
    public class GetDiscountPercentageDTO
    {
        public string copon { get; set; }
        /// <summary>
        ///ar or en
        /// </summary>
        /// <example>ar</example>
        public string lang { get; set; } = "ar";
    }

    public class FillDataDTO
    {
        public int fromBranch { get; set; }
        public int toBranch { get; set; }
        /// <summary>
        ///ar or en
        /// </summary>
        /// <example>ar</example>
        public string lang { get; set; } = "ar";
    }

    public class GetankAccountsDTO
    {
        /// <summary>
        ///ar or en
        /// </summary>
        /// <example>ar</example>
        public string lang { get; set; } = "ar";
    }
}
