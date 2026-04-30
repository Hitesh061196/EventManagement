using EventManagement.Models;

namespace EventManagement.ViewModels
{
    public class ProviderOrderDetailViewModel
    {
        public BookingServiceDetail BookingService { get; set; } = new();
        public IReadOnlyList<EventStatusMaster> StatusUpdates { get; set; } = Array.Empty<EventStatusMaster>();
    }
}
