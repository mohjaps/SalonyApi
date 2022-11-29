using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Core.TableDb
{
    public class ServiceDelivery
    {
        [Key]
        public int Id { get; set; }
        public double FromInKM { get; set; }
        public double ToInKM { get; set; }
        public double DeliveryPrice { get; set; }

        [ForeignKey(nameof(Service))]
        public int? ServiceId { get; set; }
        public MainServices Service { get; set; }
    }
}
