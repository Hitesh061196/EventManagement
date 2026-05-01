using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EventManagement.Models
{
    [Table("Work_Stage_Master")]
    public class WorkStageMaster
    {
        [Key]
        public int Work_Stage_Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Stage_Name { get; set; } = string.Empty;

        public int Stage_Order { get; set; }

        public bool Is_Active { get; set; } = true;
    }
}
