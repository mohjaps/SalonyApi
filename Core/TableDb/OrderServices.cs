using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Core.TableDb
{
    public class OrderServices
    {
        [Key]
        public int ID { get; set; }
        public int mainServiceID { get; set; }
        public string mainServiceNameAr { get; set; }
        public string mainServiceNameEn { get; set; }
        public int SubServiceID { get; set; }
        public string SubServicNameAr { get; set; }
        public string SubServicNameEn { get; set; }
        public double duration { get; set; }
        public double price { get; set; }
        //Ekelil
        public double deliveryPrice { get; set; }

        // Branch >> Show
        public double priceAtHome { get; set; }
        public double taxOfHome { get; set; }

        public string Image { get; set; }
        public string DescriptionAr { get; set; }
        public string DescriptionEn { get; set; }
        public DateTime date { get; set; }
        public string address { get; set; }
        public string lat { get; set; }
        public string lng { get; set; }
        public int EmployeeId { get; set; }
        public string EmployeeNameAr { get; set; }
        public string EmployeeNameEn { get; set; }
        public string EmployeeImg { get; set; }
        public string note { get; set; }


        public int FK_OrderID { get; set; }
        [Required]
        [ForeignKey(nameof(FK_OrderID))]
        [InverseProperty(nameof(TableDb.Orders.OrderServices))]
        public virtual Orders FK_Order { get; set; }

    }
}
