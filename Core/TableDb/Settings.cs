using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Core.TableDb
{
    public class Settings
    {
        [Key]
        public int ID { get; set; }
        public string keyMap { get; set; }

        public string aboutAppAr { get; set; }
        public string aboutAppEn { get; set; }
        public string condtionsAr { get; set; }
        public string condtionsEn { get; set; }
        public string paymentPolicyAr { get; set; }
        public string paymentPolicyEn { get; set; }
        public string appleStoreUrl { get; set; }
        public string googlePlayUrl { get; set; }
        public string phone { get; set; }
        public string phone2 { get; set; }
        public string facebook { get; set; }
        public string twitter { get; set; }
        public string telegram { get; set; }
        public string instagram { get; set; }
        public string whatsApp { get; set; }
        public string snapChat { get; set; }
        public string youtube { get; set; }
        public string commercialRegister { get; set; }
        public double appPrecent { get; set; } = 0;

        // for ToGo Branch
        public double appPrecentPercentage { get; set; } = 0;
        
        //public double appPrecentForDelivery { get; set; } = 0;
        public double Deposit { get; set; } = 0;
        
        public double Tax { get; set; } = 0;

        // for Show Branch
        public double TaxOfHome { get; set; } = 0;

        public string payText { get; set; } = "";
        public int ExpireTime { get; set; } // in minutes

        public string Screen1TitleAr { get; set; }
        public string Screen1TitleEn { get; set; }
        public string Screen2TitleAr { get; set; }
        public string Screen2TitleEn { get; set; }
        public string Screen3TitleAr { get; set; }
        public string Screen3TitleEn { get; set; }
        public string Screen1DescriptionAr { get; set; }
        public string Screen1DescriptionEn { get; set; }
        public string Screen2DescriptionAr { get; set; }
        public string Screen2DescriptionEn { get; set; }
        public string Screen3DescriptionAr { get; set; }
        public string Screen3DescriptionEn { get; set; }

        public int FK_BranchID { get; set; } //branch


        public double invitationCodeBallance { get; set; }


    }
}
