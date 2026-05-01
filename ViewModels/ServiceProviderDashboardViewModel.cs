using EventManagement.Models;

namespace EventManagement.ViewModels
{
    public class ServiceProviderDashboardViewModel
    {
        public IReadOnlyList<ServiceCatalogItem> Services { get; set; } = Array.Empty<ServiceCatalogItem>();
        public IReadOnlyList<BookingServiceDetail> Orders { get; set; } = Array.Empty<BookingServiceDetail>();

        // Maps Booking_Service_Detail_Id → current stage name
        public Dictionary<int, string> OrderCurrentStages { get; set; } = new();
    }
}
