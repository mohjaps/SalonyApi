using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Core.TableDb
{
    public class Messages
    {
        [Key]
        public int Id { get; set; }
        public int FK_OrderId { get; set; }
        public string SenderId { get; set; }
        public string ReceiverId { get; set; }
        public string Text { get; set; }
        public DateTime DateSend { get; set; }
        public bool SenderSeen { get; set; }
        public bool ReceiverSeen { get; set; }
        public bool IsDeletedSender { get; set; }
        public bool IsDeletedReceiver { get; set; }
        public int Duration { get; set; }
        public bool Closed { get; set; }


        public int TypeMessage { get; set; }

        [ForeignKey(nameof(SenderId))]
        [InverseProperty(nameof(TableDb.ApplicationDbUser.sender_Messages))]
        public virtual ApplicationDbUser Sender { get; set; }
        [Required]
        [ForeignKey(nameof(ReceiverId))]
        [InverseProperty(nameof(TableDb.ApplicationDbUser.receiver_Messages))]
        public virtual ApplicationDbUser Receiver { get; set; }
        [ForeignKey(nameof(FK_OrderId))]
        [InverseProperty(nameof(TableDb.Orders.Messages))]
        public virtual Orders FK_Order { get; set; }

    }

}
