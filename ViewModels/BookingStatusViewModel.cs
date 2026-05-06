using EventManagement.Models;

namespace EventManagement.ViewModels
{
    public class BookingStatusViewModel
    {
        public BookingCartDetail Booking { get; set; } = new();
        public IReadOnlyList<BookingServiceDetail> Services { get; set; } = Array.Empty<BookingServiceDetail>();
        public IReadOnlyList<EventStatusMaster> StatusUpdates { get; set; } = Array.Empty<EventStatusMaster>();
        public IReadOnlyList<PaymentDetail> Payments { get; set; } = Array.Empty<PaymentDetail>();

        /// <summary>
        /// Dynamically computed from event date and stored From_Time / To_Time strings.
        /// Only meaningful once the booking is paid (manager approval + payment both done).
        /// </summary>
        public string? EventTimingStatus
        {
            get
            {
                // Only show timing status for fully paid bookings
                if (Booking.Booking_Status != AppConstants.BookingStatus.Paid &&
                    Booking.Payment_Status  != AppConstants.PaymentStatus.Paid)
                    return null;

                var now       = DateTime.Now;
                var eventDate = Booking.Date.Date;

                // Fall back to start-of-day / end-of-day when time strings are absent or unparseable
                var start = ResolveTime(eventDate, Booking.From_Time) ?? eventDate;
                var end   = ResolveTime(eventDate, Booking.To_Time)   ?? eventDate.AddDays(1).AddTicks(-1);

                if (now > end)    return AppConstants.EventTimingStatus.Completed;
                if (now >= start) return AppConstants.EventTimingStatus.InProgress;
                return AppConstants.EventTimingStatus.Upcoming;
            }
        }

        // Combines a date with a stored time string ("HH:mm", "h:mm tt", etc.)
        private static DateTime? ResolveTime(DateTime date, string? timeStr)
        {
            if (string.IsNullOrWhiteSpace(timeStr)) return null;

            // "HH:mm" or "H:mm" (24-hour) handled by TimeSpan.TryParse
            if (TimeSpan.TryParse(timeStr.Trim(), out var ts))
                return date.Add(ts);

            // "h:mm tt" / "hh:mm tt" (12-hour with AM/PM) handled by DateTime.TryParse
            if (DateTime.TryParse(timeStr.Trim(), out var dt))
                return date.Add(dt.TimeOfDay);

            return null;
        }
    }
}
