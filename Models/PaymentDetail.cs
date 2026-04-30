using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EventManagement.Models
{
    [Table("Payment_Detail")]
    public class PaymentDetail
    {
        [Key]
        public int Payment_Id { get; set; }

        public int Booking_Id_fk { get; set; }

        [Column(TypeName = "decimal(15,2)")]
        public decimal Amount { get; set; }

        [Required]
        [StringLength(50)]
        public string Payment_Method { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string Payment_Status { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string Transaction_Reference { get; set; } = string.Empty;

        public DateTime Paid_On { get; set; }

        [ForeignKey(nameof(Booking_Id_fk))]
        public virtual BookingCartDetail? Booking { get; set; }
    }
}
