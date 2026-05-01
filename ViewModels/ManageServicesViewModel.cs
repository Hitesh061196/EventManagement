using System.ComponentModel.DataAnnotations;
using EventManagement.Models;

namespace EventManagement.ViewModels
{
    public class ManageServicesViewModel
    {
        [Required]
        [StringLength(80)]
        public string ServiceName { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string ServiceCategory { get; set; } = string.Empty;

        [Required]
        public int EventTypeId { get; set; }

        [Range(1, 1000000)]
        public decimal Price { get; set; }

        [Required]
        [StringLength(30)]
        public string PriceType { get; set; } = "For All";

        [StringLength(500)]
        public string PhotoUrl { get; set; } = string.Empty;

        public IFormFile? PhotoFile { get; set; }

        [Required]
        [StringLength(1000)]
        public string Description { get; set; } = string.Empty;

        public IReadOnlyList<ServiceCatalogItem> ExistingServices { get; set; } = Array.Empty<ServiceCatalogItem>();
        public IReadOnlyList<EventTypeMaster> EventTypes { get; set; } = Array.Empty<EventTypeMaster>();
        public string ProviderCategory { get; set; } = string.Empty;
        public IReadOnlyList<string> PriceTypes { get; set; } = AppConstants.PriceTypes;
    }
}
