
using Core.TableDb;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.TableDb
{
    public class ProviderAditionalData
    {
        [Key]
        public int ID { get; set; }
        public string nameAr { get; set; }
        public string nameEn { get; set; }
        public string address { get; set; }
        public string lat { get; set; }
        public string lng { get; set; }
        public DateTime timeForm { get; set; }
        public DateTime timeTo { get; set; }
        public DateTime timeFormEvening { get; set; }
        public DateTime timeToEvening { get; set; }
        public string dayWorks { get; set; } = null; //"1,2,3,4,5"
        public int salonType { get; set; } = 3; //SalonType
        public int SalonUsersType { get; set; } = 3; //SalonUsersType 
        public string descriptionAr { get; set; }
        public string descriptionEn { get; set; }
        public string bankAccount { get; set; } = null;
        public int rate { get; set; }
        public string socialMediaProfile { get; set; } = null;

        public DateTime lastPayDate { get; set; } = DateTime.Now;
        public double paied { get; set; } = 0;


        public string IdentityImage { get; set; }
        public string commercialRegister { get; set; }
        public string CommercialRegisterImage { get; set; }
        public string HealthCardImage { get; set; }
        public string IbanNumber { get; set; }
        public string IbanImage { get; set; }




        public string FK_UserID { get; set; }
        public int FK_DistrictID { get; set; }
        public int FK_CategoryID { get; set; }

        [ForeignKey(nameof(FK_UserID))]
        [InverseProperty(nameof(ApplicationDbUser.ProviderAditionalData))]
        public virtual ApplicationDbUser FK_User { get; set; }

        [ForeignKey(nameof(FK_DistrictID))]
        [InverseProperty(nameof(TableDb.Districts.ProviderAditionalData))]
        public virtual Districts FK_District { get; set; }

        [ForeignKey(nameof(FK_CategoryID))]
        [InverseProperty(nameof(TableDb.Categories.ProviderAditionalData))]
        public virtual Categories FK_Category { get; set; }

        [InverseProperty(nameof(TableDb.SubServices.FK_ProviderAdditionalData))]
        public virtual ICollection<SubServices> SubServices { get; set; } = new HashSet<SubServices>();
        [InverseProperty(nameof(TableDb.Offers.FK_ProviderAdditionalData))]
        public virtual ICollection<Offers> Offers { get; set; } = new HashSet<Offers>();
        [InverseProperty(nameof(TableDb.Orders.FK_Provider))]
        public virtual ICollection<Orders> Orders { get; set; } = new HashSet<Orders>();
        [InverseProperty(nameof(TableDb.SalonImages.FK_ProviderAdditionalData))]
        public virtual ICollection<SalonImages> SalonImages { get; set; } = new HashSet<SalonImages>();
        [InverseProperty(nameof(TableDb.Employees.FK_ProviderAdditionalData))]
        public virtual ICollection<Employees> Employees { get; set; } = new HashSet<Employees>();
        public string BankName { get; set; }
        public string IdNumber { get; set; }
    }
}