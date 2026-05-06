namespace EventManagement
{
    public static class AppConstants
    {
        // ── Approval / provider statuses ───────────────────────────────────────
        public static class ApprovalStatus
        {
            public const string Pending  = "Pending";
            public const string Approved = "Approved";
            public const string Rejected = "Rejected";
        }

        // ── Booking statuses ───────────────────────────────────────────────────
        public static class BookingStatus
        {
            public const string PendingManagerApproval = "Pending Event Manager Approval";
            public const string ConfirmedByManager     = "Confirmed by Event Manager";
            public const string RejectedByManager      = "Rejected by Event Manager";
            public const string Paid                   = "Paid";
            public const string Cancelled              = "Cancelled";
        }

        // ── Payment statuses ───────────────────────────────────────────────────
        public static class PaymentStatus
        {
            public const string Pending = "Pending";
            public const string Paid    = "Paid";
        }

        // ── Payment methods ────────────────────────────────────────────────────
        public static class PaymentMethod
        {
            public const string Upi        = "UPI";
            public const string CreditCard = "Credit Card";
            public const string NetBanking = "Net Banking";
        }

        // ── Service-line confirmation statuses ─────────────────────────────────
        public static class ConfirmationStatus
        {
            public const string Pending   = "Pending";
            public const string Confirmed = "Confirmed";
        }

        // ── Event type statuses ────────────────────────────────────────────────
        public static class EventTypeStatus
        {
            public const string Active   = "Active";
            public const string Inactive = "Inactive";
        }

        // ── Event timing statuses (computed from date/time, not stored) ────────
        public static class EventTimingStatus
        {
            public const string Upcoming   = "Upcoming";
            public const string InProgress = "In Progress";
            public const string Completed  = "Completed";
        }

        // ── Role names (must match Roll_Detail seeds) ──────────────────────────
        public static class Roles
        {
            public const string Admin           = "Admin";
            public const string Customer        = "Customer";
            public const string EventManager    = "Event Manager";
            public const string EventPlanner    = "Event Planner";
            public const string ServiceProvider = "Service Provider";

            public static readonly IReadOnlyList<string> StaffRoles = new[]
            {
                EventManager, EventPlanner
            };
        }

        // ── Password generation ────────────────────────────────────────────────
        private static readonly char[] PasswordChars =
            "abcdefghjkmnpqrstuvwxyzABCDEFGHJKMNPQRSTUVWXYZ23456789".ToCharArray();

        public static string GeneratePassword(int length = 10)
        {
            var rng = System.Security.Cryptography.RandomNumberGenerator.Create();
            var bytes = new byte[length];
            rng.GetBytes(bytes);
            return new string(bytes.Select(b => PasswordChars[b % PasswordChars.Length]).ToArray());
        }
    }
}
