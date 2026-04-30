using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EventManagement.Models
{
    [Table("Booking_Service_Detail")]
    public class BookingServiceDetail
    {
        [Key]
        public int Booking_Service_Detail_Id { get; set; }

        public int Booking_Id_fk { get; set; }

        public int Service_Catalog_Item_Id_fk { get; set; }

        [Required]
        [StringLength(50)]
        public string Service_Category { get; set; } = string.Empty;

        [Column(TypeName = "decimal(15,2)")]
        public decimal Price { get; set; }

        [StringLength(50)]
        public string Confirmation_Status { get; set; } = "Pending";

        [StringLength(200)]
        public string Provider_Comment { get; set; } = string.Empty;

        public DateTime? Confirmed_On { get; set; }

        [ForeignKey(nameof(Booking_Id_fk))]
        public virtual BookingCartDetail? Booking { get; set; }

        [ForeignKey(nameof(Service_Catalog_Item_Id_fk))]
        public virtual ServiceCatalogItem? ServiceCatalogItem { get; set; }
    }
}
