namespace EventManagement.ViewModels
{
    public class AdminDashboardViewModel
    {
        public int PendingProviders { get; set; }
        public int ActiveProviders { get; set; }
        public int PendingBookings { get; set; }
        public int FeedbackCount { get; set; }
        public int InquiryCount { get; set; }
        public int EventManagerCount { get; set; }
    }
}
