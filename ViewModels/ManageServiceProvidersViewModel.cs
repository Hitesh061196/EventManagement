using EventManagement.Models;

namespace EventManagement.ViewModels
{
    public class ManageServiceProvidersViewModel
    {
        public IReadOnlyList<EventManagement.Models.ServiceProvider> PendingProviders { get; set; } = Array.Empty<EventManagement.Models.ServiceProvider>();
        public IReadOnlyList<EventManagement.Models.ServiceProvider> ApprovedProviders { get; set; } = Array.Empty<EventManagement.Models.ServiceProvider>();
        public IReadOnlyList<EventManagement.Models.ServiceProvider> RejectedProviders { get; set; } = Array.Empty<EventManagement.Models.ServiceProvider>();
        public IReadOnlyList<AreaMaster> Areas { get; set; } = Array.Empty<AreaMaster>();
    }
}
