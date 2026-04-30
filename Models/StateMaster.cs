using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EventManagement.Models
{
    [Table("State_Master")]
    public class StateMaster
    {
        [Key]
        public int State_Id { get; set; }

        [Required]
        [StringLength(50)]
        public string State_Name { get; set; } = string.Empty;

        public bool State_Status { get; set; } = true;

        public virtual ICollection<CityMaster> Cities { get; set; } = new List<CityMaster>();
    }
}
