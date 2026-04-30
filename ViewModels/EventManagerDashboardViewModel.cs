using EventManagement.Models;

namespace EventManagement.ViewModels
{
    public class EventManagerDashboardViewModel
    {
        public IReadOnlyList<BookingCartDetail> Bookings { get; set; } = Array.Empty<BookingCartDetail>();
        public IReadOnlyList<BookingServiceDetail> BookingServices { get; set; } = Array.Empty<BookingServiceDetail>();
    }
}
