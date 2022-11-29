using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Core.TableDb
{
    public class DeviceIds
    {
        [Key]
        public int ID { get; set; }
        public string deviceID { get; set; }
        public string deviceType { get; set; } = "android"; // android or ios
        public DateTime date { get; set; }

        public string FK_UserID { get; set; }
        [Required]
        [ForeignKey(nameof(FK_UserID))]
        [InverseProperty(nameof(TableDb.ApplicationDbUser.DeviceIds))]
        public virtual ApplicationDbUser FK_User { get; set; }

    }
}
