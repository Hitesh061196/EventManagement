namespace EventManagement.ViewModels
{
    public class ServiceOptionViewModel
    {
        public int Id { get; set; }
        public int EventTypeId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string Description { get; set; } = string.Empty;
        public string PhotoUrl { get; set; } = string.Empty;
        public string ProviderName { get; set; } = string.Empty;
        public bool IsSelected { get; set; }
    }
}
