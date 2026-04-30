using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EventManagement.Models
{
    [Table("FeedBack_Detail")]
    public class FeedBackDetail
    {
        [Key]
        public int FeedBack_Id { get; set; }
        [StringLength(15)]
        public string Name { get; set; } = string.Empty;
        [StringLength(30)]
        public string Email { get; set; } = string.Empty;

        public string FeedBack { get; set; } = string.Empty;

        public bool FeedBack_Status { get; set; }

        public DateTime Created_On { get; set; } = DateTime.UtcNow;
    }
}

