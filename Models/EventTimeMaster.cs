using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EventManagement.Models
{
    [Table("Event_Time_Master")]
    public class EventTimeMaster
    {
        [Key]
        public int Event_Time_Id { get; set; }

        [Required]
        [StringLength(30)]
        public string Event_Time_Name { get; set; } = string.Empty;

        public bool Is_Active { get; set; } = true;
    }
}
