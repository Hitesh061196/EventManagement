using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EventManagement.Models
{
    [Table("Service_Provider_Type_Master")]
    public class ServiceProviderTypeMaster
    {
        [Key]
        public int Provider_Type_Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Type_Name { get; set; } = string.Empty;

        public bool Is_Active { get; set; } = true;
    }
}
