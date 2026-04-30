using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EventManagement.Models
{
    [Table("Service_Catalog_Item")]
    public class ServiceCatalogItem
    {
        [Key]
        public int Service_Catalog_Item_Id { get; set; }

        [Required]
        [StringLength(80)]
        public string Service_Name { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string Service_Category { get; set; } = string.Empty;

        public int Event_Type_Id_fk { get; set; }

        public int? Service_Provider_Id_fk { get; set; }

        [Column(TypeName = "decimal(15,2)")]
        public decimal Price { get; set; }

        [StringLength(500)]
        public string Photo_Url { get; set; } = string.Empty;

        [StringLength(1000)]
        public string Description { get; set; } = string.Empty;

        public bool Is_Active { get; set; } = true;

        [ForeignKey(nameof(Event_Type_Id_fk))]
        public virtual EventTypeMaster? EventType { get; set; }

        [ForeignKey(nameof(Service_Provider_Id_fk))]
        public virtual ServiceProvider? ServiceProvider { get; set; }

        public virtual ICollection<BookingServiceDetail> BookingServices { get; set; } = new List<BookingServiceDetail>();
    }
}
