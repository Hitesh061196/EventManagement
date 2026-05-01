using System.ComponentModel.DataAnnotations;
using EventManagement.Models;

namespace EventManagement.ViewModels
{
    public class CreateEventManagerViewModel
    {
        [Required]
        [StringLength(50)]
        public string FirstName { get; set; } = string.Empty;

        [StringLength(50)]
        public string MiddleName { get; set; } = string.Empty;

        [StringLength(50)]
        public string LastName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [StringLength(10)]
        public string ContactNo { get; set; } = string.Empty;

        [StringLength(10)]
        public string Gender { get; set; } = "Male";

        [StringLength(100)]
        public string Address { get; set; } = string.Empty;

        public int StateId { get; set; }
        public int CityId { get; set; }

        [Required]
        public int AreaId { get; set; }

        [Required]
        public string RoleName { get; set; } = "Event Manager";

        public IReadOnlyList<StateMaster> States { get; set; } = Array.Empty<StateMaster>();
        public IReadOnlyList<AreaMaster> Areas { get; set; } = Array.Empty<AreaMaster>();
        public IReadOnlyList<UserRegistrationDetail> ExistingStaff { get; set; } = Array.Empty<UserRegistrationDetail>();
        public IReadOnlyList<RollDetail> AvailableRoles { get; set; } = Array.Empty<RollDetail>();
    }
}
