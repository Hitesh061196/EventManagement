using EventManagement.Models;

namespace EventManagement.ViewModels
{
    public class ServiceProviderDashboardViewModel
    {
        public IReadOnlyList<ServiceCatalogItem> Services { get; set; } = Array.Empty<ServiceCatalogItem>();
        public IReadOnlyList<BookingServiceDetail> Orders { get; set; } = Array.Empty<BookingServiceDetail>();
    }
}
