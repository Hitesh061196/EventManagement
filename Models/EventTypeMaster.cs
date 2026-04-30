using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EventManagement.Models
{
    [Table("Event_Type_Master")]
    public class EventTypeMaster
    {
        [Key]
        public int Event_Id { get; set; }
        [Required]
        [StringLength(50)]
        public string Event_Type { get; set; } = string.Empty;
        [StringLength(50)]
        public string Event_Type_Status { get; set; } = string.Empty;

        public virtual ICollection<ServiceCatalogItem> Services { get; set; } = new List<ServiceCatalogItem>();
    }
}

