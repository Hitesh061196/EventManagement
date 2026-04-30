using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EventManagement.Models
{
    [Table("City_Master")]
    public class CityMaster
    {
        [Key]
        public int City_Id { get; set; }

        [Required]
        [StringLength(50)]
        public string City_Name { get; set; } = string.Empty;

        public bool City_Status { get; set; }

        public int State_Id_fk { get; set; }

        [ForeignKey(nameof(State_Id_fk))]
        public virtual StateMaster? State { get; set; }

        public virtual ICollection<AreaMaster> Areas { get; set; } = new List<AreaMaster>();
    }
}

