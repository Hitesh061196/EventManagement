using System.ComponentModel.DataAnnotations;
using EventManagement.Models;

namespace EventManagement.ViewModels
{
    public class CustomerBookingViewModel : IValidatableObject
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

        [Required]
        [StringLength(30)]
        public string EventTime { get; set; } = "Dinner";

        [StringLength(10)]
        public string FromTime { get; set; } = string.Empty;

        [StringLength(10)]
        public string ToTime { get; set; } = string.Empty;

        [StringLength(200)]
        public string Comment { get; set; } = string.Empty;

        public IReadOnlyList<EventTypeMaster> EventTypes { get; set; } = Array.Empty<EventTypeMaster>();
        public IReadOnlyList<EventTimeMaster> EventTimes { get; set; } = Array.Empty<EventTimeMaster>();
        public IReadOnlyList<ServiceOptionViewModel> PartyPlots { get; set; } = Array.Empty<ServiceOptionViewModel>();
        public IReadOnlyList<ServiceOptionViewModel> Caterers { get; set; } = Array.Empty<ServiceOptionViewModel>();
        public IReadOnlyList<ServiceOptionViewModel> Decorations { get; set; } = Array.Empty<ServiceOptionViewModel>();
        public IReadOnlyList<ServiceOptionViewModel> Musics { get; set; } = Array.Empty<ServiceOptionViewModel>();
        public IReadOnlyList<ServiceOptionViewModel> Transporters { get; set; } = Array.Empty<ServiceOptionViewModel>();

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (Date.Date < DateTime.Today)
                yield return new ValidationResult(
                    "Event date must be today or a future date.",
                    new[] { nameof(Date) });

            if (!string.IsNullOrWhiteSpace(FromTime) && !string.IsNullOrWhiteSpace(ToTime))
            {
                if (TimeSpan.TryParse(FromTime, out var from) && TimeSpan.TryParse(ToTime, out var to) && to <= from)
                    yield return new ValidationResult(
                        "To Time must be later than From Time.",
                        new[] { nameof(ToTime) });
            }
        }
    }
}
