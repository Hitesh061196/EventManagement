using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EventManagement.Models
{
    [Table("Event_Status_Master")]
    public class EventStatusMaster
    {
        [Key]
        public int Event_Status_Id { get; set; }
        public int Service_Person_Id_fk { get; set; }
        public int Event_Booking_Cart_fk { get; set; }
        public int Event_Type_Id_fk { get; set; }
        public bool Status { get; set; }
        public DateTime Date { get; set; }

        [StringLength(500)]
        public string Photo { get; set; } = string.Empty;

        [StringLength(1000)]
        public string Description { get; set; } = string.Empty;

        [StringLength(100)]
        public string Event_Type { get; set; } = string.Empty;

        [ForeignKey(nameof(Service_Person_Id_fk))]
        public virtual LoginDetail? ServicePersonLogin { get; set; }

        [ForeignKey(nameof(Event_Booking_Cart_fk))]
        public virtual BookingCartDetail? Booking { get; set; }
    }
}

