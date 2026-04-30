using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EventManagement.Models
{
    [Table("Area_Master")]
    public class AreaMaster
    {
        [Key]
        public int Area_Id { get; set; }
        [Required]
        [StringLength(50)]
        public string Area_Name { get; set; } = string.Empty;

        public bool Area_Status { get; set; }

        public int City_Id_fk { get; set; }

        [ForeignKey(nameof(City_Id_fk))]
        public virtual CityMaster? City { get; set; }

        public virtual ICollection<UserRegistrationDetail> Users { get; set; } = new List<UserRegistrationDetail>();
    }
}

