using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EventManagement.Models
{
    [Table("Service_Provider")]
    public class ServiceProvider
    {
        [Key]
        public int Service_Provider_Id { get; set; }
        [Required]
        [StringLength(50)]
        public string Service_Provider_Name { get; set; } = string.Empty;
        [StringLength(30)]
        public string Email_Id { get; set; } = string.Empty;

        [StringLength(15)]
        public string Mobile_No { get; set; } = string.Empty;

        [StringLength(100)]
        public string Shop_Name { get; set; } = string.Empty;

        [StringLength(10)]
        public string Gender { get; set; } = string.Empty;

        [StringLength(50)]
        public string Address { get; set; } = string.Empty;
        public int Service_Provider_Area_Id_fk { get; set; }
        [StringLength(50)]
        public string Service_Provider_Type { get; set; } = string.Empty;

        public int Login_Id_fk { get; set; }

        [StringLength(30)]
        public string Approval_Status { get; set; } = "Pending";

        public bool Is_Blocked { get; set; }

        [StringLength(200)]
        public string Rejection_Reason { get; set; } = string.Empty;

        [StringLength(500)]
        public string Profile_Image_Url { get; set; } = string.Empty;

        [StringLength(1000)]
        public string Description { get; set; } = string.Empty;

        [ForeignKey(nameof(Service_Provider_Area_Id_fk))]
        public virtual AreaMaster? Area { get; set; }

        [ForeignKey(nameof(Login_Id_fk))]
        public virtual LoginDetail? Login { get; set; }

        public virtual ICollection<ServiceCatalogItem> Services { get; set; } = new List<ServiceCatalogItem>();
    }
}

