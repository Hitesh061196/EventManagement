using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EventManagement.Models
{
    [Table("Inquiry_Detail")]
    public class InquiryDetail
    {
        [Key]
        public int Inquiry_Id { get; set; }
        [StringLength(50)]
        public string Name { get; set; } = string.Empty;
        [StringLength(30)]
        public string Email { get; set; } = string.Empty;
        [StringLength(250)]
        public string Question { get; set; } = string.Empty;
        public bool Inquiry_Status { get; set; }
        public DateTime Date { get; set; }
        [StringLength(500)]
        public string Response { get; set; } = string.Empty;
        public DateTime Response_Date { get; set; }
    }
}

