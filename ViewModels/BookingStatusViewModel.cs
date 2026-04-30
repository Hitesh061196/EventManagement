using EventManagement.Models;

namespace EventManagement.ViewModels
{
    public class BookingStatusViewModel
    {
        public BookingCartDetail Booking { get; set; } = new();
        public IReadOnlyList<BookingServiceDetail> Services { get; set; } = Array.Empty<BookingServiceDetail>();
        public IReadOnlyList<EventStatusMaster> StatusUpdates { get; set; } = Array.Empty<EventStatusMaster>();
        public IReadOnlyList<PaymentDetail> Payments { get; set; } = Array.Empty<PaymentDetail>();
    }
}
