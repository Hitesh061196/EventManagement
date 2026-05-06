using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EventManagement.Models
{
    [Table("Booking_Cart_Detail")]
    public class BookingCartDetail
    {
        [Key]
        public int Booking_Id { get; set; }

        public int Event_Package_Id_fk { get; set; }

        public int Custom_Id_fk { get; set; }

        public DateTime Date { get; set; }

        [Column(TypeName = "decimal(15,2)")]
        public decimal Ammount { get; set; }

        public bool Status { get; set; }

        [StringLength(50)]
        public string Comment { get; set; } = string.Empty;

        public int Guest_Count { get; set; }

        [StringLength(40)]
        public string Booking_Reference { get; set; } = string.Empty;

        [StringLength(40)]
        public string Booking_Status { get; set; } = "Pending";

        [StringLength(40)]
        public string Payment_Status { get; set; } = "Pending";

        [StringLength(30)]
        public string Event_Time { get; set; } = string.Empty;

        [StringLength(10)]
        public string From_Time { get; set; } = string.Empty;

        [StringLength(10)]
        public string To_Time { get; set; } = string.Empty;

        public bool Event_Manager_Approved { get; set; }

        [StringLength(200)]
        public string? Rejection_Reason { get; set; }

        public int? Event_Manager_Login_Id_fk { get; set; }

        public DateTime Created_On { get; set; } = DateTime.UtcNow;

        [ForeignKey(nameof(Event_Package_Id_fk))]
        public virtual EventTypeMaster? EventType { get; set; }

        [ForeignKey(nameof(Custom_Id_fk))]
        public virtual UserRegistrationDetail? Customer { get; set; }

        [ForeignKey(nameof(Event_Manager_Login_Id_fk))]
        public virtual LoginDetail? EventManagerLogin { get; set; }

        public virtual ICollection<BookingServiceDetail> Services { get; set; } = new List<BookingServiceDetail>();

        public virtual ICollection<EventStatusMaster> StatusUpdates { get; set; } = new List<EventStatusMaster>();

        public virtual ICollection<PaymentDetail> Payments { get; set; } = new List<PaymentDetail>();
    }
}

