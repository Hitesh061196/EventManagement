using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EventManagement.Models
{
    [Table("User_Registration_Detail")]
    public class UserRegistrationDetail
    {
        [Key]
        public int User_Id { get; set; }
        [Required]
        [StringLength(50)]
        public string First_Name { get; set; } = string.Empty;

        [StringLength(50)]
        public string Last_Name { get; set; } = string.Empty;

        [StringLength(50)]
        public string Address { get; set; } = string.Empty;

        [StringLength(10)]
        public string Contact_No { get; set; } = string.Empty;

        [StringLength(30)]
        public string Email_Id { get; set; } = string.Empty;

        [StringLength(10)]
        public string Gender { get; set; } = string.Empty;

        public int Area_Id_fk { get; set; }

        public int Login_Id_fk { get; set; }

        [ForeignKey(nameof(Area_Id_fk))]
        public virtual AreaMaster? Area { get; set; }

        [ForeignKey(nameof(Login_Id_fk))]
        public virtual LoginDetail? Login { get; set; }
    }
}

