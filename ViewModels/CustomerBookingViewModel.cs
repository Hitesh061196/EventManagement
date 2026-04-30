using System.ComponentModel.DataAnnotations;
using EventManagement.Models;

namespace EventManagement.ViewModels
{
    public class CustomerBookingViewModel
    {
        [Range(1, int.MaxValue)]
        public int EventTypeId { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime Date { get; set; } = DateTime.Today.AddDays(7);

        [Range(10, 5000)]
        public int GuestCount { get; set; } = 100;

        [Range(1, int.MaxValue)]
        public int PartyPlotId { get; set; }

        [Range(1, int.MaxValue)]
        public int CatererId { get; set; }

        public int? DecorationId { get; set; }

        public int? MusicId { get; set; }

        [Range(1, int.MaxValue)]
        public int TransporterId { get; set; }

        [StringLength(200)]
        public string Comment { get; set; } = string.Empty;

        public IReadOnlyList<EventTypeMaster> EventTypes { get; set; } = Array.Empty<EventTypeMaster>();
        public IReadOnlyList<ServiceOptionViewModel> PartyPlots { get; set; } = Array.Empty<ServiceOptionViewModel>();
        public IReadOnlyList<ServiceOptionViewModel> Caterers { get; set; } = Array.Empty<ServiceOptionViewModel>();
        public IReadOnlyList<ServiceOptionViewModel> Decorations { get; set; } = Array.Empty<ServiceOptionViewModel>();
        public IReadOnlyList<ServiceOptionViewModel> Musics { get; set; } = Array.Empty<ServiceOptionViewModel>();
        public IReadOnlyList<ServiceOptionViewModel> Transporters { get; set; } = Array.Empty<ServiceOptionViewModel>();
    }
}
