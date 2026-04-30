using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EventManagement.Models
{
    [Table("Roll_Detail")]
    public class RollDetail
    {
        [Key]
        public int Roll_Id { get; set; }
        [Required]
        [StringLength(50)]
        public string Roll_Name { get; set; } = string.Empty;
        public bool Status { get; set; }
    }
}

