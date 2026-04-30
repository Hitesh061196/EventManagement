using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EventManagement.Models
{
    [Table("Login_Detail")]
    public class LoginDetail
    {
        [Key]
        public int Login_Id { get; set; }
        [Required]
        [StringLength(50)]
        public string User_Name { get; set; } = string.Empty;
        [Required]
        [StringLength(20)]
        public string Password { get; set; } = string.Empty;

        public int Roll_Id_fk { get; set; }

        public bool Is_Active { get; set; } = true;

        public bool Must_Change_Password { get; set; }

        [StringLength(250)]
        public string Last_Notification { get; set; } = string.Empty;

        [ForeignKey(nameof(Roll_Id_fk))]
        public virtual RollDetail? Roll { get; set; }
    }
}

