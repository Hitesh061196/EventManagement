using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EventManagement.Models
{
    [Table("Price_Type_Master")]
    public class PriceTypeMaster
    {
        [Key]
        public int Price_Type_Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Price_Type_Name { get; set; } = string.Empty;

        public int Sort_Order { get; set; }

        public bool Is_Active { get; set; } = true;
    }
}
