using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EventManagement.Models;
using EventManagement.ViewModels;

namespace EventManagement.Controllers
{
    public class CustomerController : Controller
    {
        private readonly EventManagementDbContext _context;

        public CustomerController(EventManagementDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return RedirectToAction(nameof(BookEvent));
        }

        public async Task<IActionResult> BookEvent()
        {
            if (!IsCustomer())
            {
                return RedirectToAction("Login", "Account");
            }

            return View(await BuildBookingViewModelAsync());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> BookEvent(CustomerBookingViewModel model)
        {
            if (!IsCustomer())
            {
                return RedirectToAction("Login", "Account");
            }

            if (!ModelState.IsValid)
            {
                return View(await BuildBookingViewModelAsync(model));
            }

            var activeServices = await _context.ServiceCatalogItems
                .Include(s => s.ServiceProvider)
                .Where(s => s.Is_Active &&
                            s.ServiceProvider != null &&
                            s.ServiceProvider.Approval_Status == AppConstants.ApprovalStatus.Approved &&
                            !s.ServiceProvider.Is_Blocked)
                .ToListAsync();

            ValidateSelection(activeServices, model.PartyPlotId,   nameof(model.PartyPlotId),   "Party Plot",  true);
            ValidateSelection(activeServices, model.CatererId,      nameof(model.CatererId),      "Caterer",     true);
            ValidateSelection(activeServices, model.DecorationId,   nameof(model.DecorationId),   "Decoration",  false);
            ValidateSelection(activeServices, model.MusicId,        nameof(model.MusicId),        "Music",       false);
            ValidateSelection(activeServices, model.TransporterId,  nameof(model.TransporterId),  "Transporter", true);

            if (!ModelState.IsValid)
            {
                return View(await BuildBookingViewModelAsync(model));
            }

            var selectedServiceIds = new[] { model.PartyPlotId, model.CatererId, model.TransporterId, model.DecorationId ?? 0, model.MusicId ?? 0 }
                .Where(id => id > 0)
                .ToList();

            var services = activeServices
                .Where(s => selectedServiceIds.Contains(s.Service_Catalog_Item_Id))
                .ToList();

            var sameDateConflict = await _context.BookingServiceDetails
                .Include(s => s.Booking)
                .AnyAsync(s => s.Service_Catalog_Item_Id_fk == model.PartyPlotId &&
                               s.Booking != null &&
                               s.Booking.Date.Date == model.Date.Date &&
                               s.Booking.Booking_Status != AppConstants.BookingStatus.Cancelled);

            if (sameDateConflict)
            {
                ModelState.AddModelError(nameof(model.PartyPlotId), "This party plot is already booked on the selected date. Please choose another party plot or another date.");
                TempData["Error"] = "Same date for not booking. Please select another party plot or date.";
                return View(await BuildBookingViewModelAsync(model));
            }

            var customerProfileId = await GetCustomerProfileIdAsync();
            var amount = services.Sum(s => s.Price);
            var booking = new BookingCartDetail
            {
                Event_Package_Id_fk = model.EventTypeId,
                Custom_Id_fk        = customerProfileId,
                Date                = model.Date,
                Guest_Count         = model.GuestCount,
                Event_Time          = model.EventTime,
                From_Time           = model.FromTime,
                To_Time             = model.ToTime,
                Ammount             = amount,
                Status              = true,
                Comment             = string.IsNullOrWhiteSpace(model.Comment) ? $"Booked for {model.GuestCount} guests" : model.Comment,
                Booking_Reference   = $"EM-{DateTime.Now:yyyyMMddHHmmss}",
                Booking_Status      = AppConstants.BookingStatus.PendingManagerApproval,
                Payment_Status      = AppConstants.PaymentStatus.Pending,
                Created_On          = DateTime.UtcNow
            };
            _context.BookingCartDetails.Add(booking);
            await _context.SaveChangesAsync();

            foreach (var service in services)
            {
                _context.BookingServiceDetails.Add(new BookingServiceDetail
                {
                    Booking_Id_fk              = booking.Booking_Id,
                    Service_Catalog_Item_Id_fk = service.Service_Catalog_Item_Id,
                    Service_Category           = service.Service_Category,
                    Price                      = service.Price,
                    Confirmation_Status        = AppConstants.ConfirmationStatus.Pending
                });
            }

            await _context.SaveChangesAsync();
            TempData["Success"] = $"Booking successfully registered. Total amount counted: {booking.Ammount:C}.";
            return RedirectToAction(nameof(TrackStatus));
        }

        public async Task<IActionResult> TrackStatus()
        {
            if (!IsCustomer())
            {
                return RedirectToAction("Login", "Account");
            }

            var customerProfileId = await GetCustomerProfileIdAsync();
            var bookings = await _context.BookingCartDetails
                .Include(b => b.EventType)
                .Include(b => b.Services).ThenInclude(s => s.ServiceCatalogItem)!.ThenInclude(s => s!.ServiceProvider)
                .Include(b => b.StatusUpdates)
                .Include(b => b.Payments)
                .Where(b => b.Custom_Id_fk == customerProfileId)
                .OrderByDescending(b => b.Date)
                .Select(b => new BookingStatusViewModel
                {
                    Booking        = b,
                    Services       = b.Services.OrderBy(s => s.Service_Category).ToList(),
                    StatusUpdates  = b.StatusUpdates.OrderByDescending(s => s.Date).ToList(),
                    Payments       = b.Payments.OrderByDescending(p => p.Paid_On).ToList()
                })
                .ToListAsync();

            return View(bookings);
        }

        public async Task<IActionResult> Payment(int id)
        {
            if (!IsCustomer())
            {
                return RedirectToAction("Login", "Account");
            }

            var customerProfileId = await GetCustomerProfileIdAsync();
            var booking = await _context.BookingCartDetails
                .Include(b => b.EventType)
                .FirstOrDefaultAsync(b => b.Booking_Id == id && b.Custom_Id_fk == customerProfileId);

            if (booking == null)
            {
                return RedirectToAction(nameof(TrackStatus));
            }

            if (!booking.Event_Manager_Approved)
            {
                TempData["Error"] = "Payment page is available once the event manager confirms the order.";
                return RedirectToAction(nameof(TrackStatus));
            }

            return View(booking);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Payment(int id, string paymentMethod)
        {
            var customerProfileId = await GetCustomerProfileIdAsync();
            var booking = await _context.BookingCartDetails
                .FirstOrDefaultAsync(b => b.Booking_Id == id && b.Custom_Id_fk == customerProfileId);

            if (booking == null)
            {
                return RedirectToAction(nameof(TrackStatus));
            }

            booking.Payment_Status = AppConstants.PaymentStatus.Paid;
            booking.Booking_Status = AppConstants.BookingStatus.Paid;

            _context.PaymentDetails.Add(new PaymentDetail
            {
                Booking_Id_fk         = booking.Booking_Id,
                Amount                = booking.Ammount,
                Payment_Method        = paymentMethod,
                Payment_Status        = AppConstants.PaymentStatus.Paid,
                Transaction_Reference = $"PAY-{Guid.NewGuid():N}"[..12].ToUpperInvariant(),
                Paid_On               = DateTime.UtcNow
            });

            await _context.SaveChangesAsync();
            TempData["Success"] = $"Payment successful for booking {booking.Booking_Reference}.";
            return RedirectToAction(nameof(TrackStatus));
        }

        public IActionResult Feedback()
        {
            if (!IsCustomer())
            {
                return RedirectToAction("Login", "Account");
            }

            return View(new FeedBackDetail());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Feedback(FeedBackDetail fb)
        {
            if (!IsCustomer())
            {
                return RedirectToAction("Login", "Account");
            }

            fb.FeedBack_Status = true;
            fb.Created_On = DateTime.UtcNow;
            _context.FeedBackDetails.Add(fb);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Feedback submitted";
            return RedirectToAction(nameof(Feedback));
        }

        private bool IsCustomer()
        {
            return HttpContext.Session.GetString("Role") == AppConstants.Roles.Customer;
        }

        private async Task<int> GetCustomerProfileIdAsync()
        {
            var loginId = int.Parse(HttpContext.Session.GetString("UserId") ?? "0");
            return await _context.UserRegistrationDetails
                .Where(u => u.Login_Id_fk == loginId)
                .Select(u => u.User_Id)
                .FirstAsync();
        }

        private async Task<CustomerBookingViewModel> BuildBookingViewModelAsync(CustomerBookingViewModel? source = null)
        {
            var eventTypes = await _context.EventTypeMasters
                .Where(e => e.Event_Type_Status == AppConstants.EventTypeStatus.Active)
                .OrderBy(e => e.Event_Type)
                .ToListAsync();

            var eventTimes = await _context.EventTimeMasters
                .Where(t => t.Is_Active)
                .OrderBy(t => t.Event_Time_Id)
                .ToListAsync();

            var services = await _context.ServiceCatalogItems
                .Include(s => s.ServiceProvider)
                .Where(s => s.Is_Active &&
                            s.ServiceProvider != null &&
                            s.ServiceProvider.Approval_Status == AppConstants.ApprovalStatus.Approved &&
                            !s.ServiceProvider.Is_Blocked)
                .OrderBy(s => s.Service_Name)
                .ToListAsync();

            return new CustomerBookingViewModel
            {
                EventTypeId  = source?.EventTypeId ?? eventTypes.FirstOrDefault()?.Event_Id ?? 0,
                Date         = source?.Date ?? DateTime.Today.AddDays(7),
                GuestCount   = source?.GuestCount ?? 100,
                PartyPlotId  = source?.PartyPlotId ?? 0,
                CatererId    = source?.CatererId ?? 0,
                DecorationId = source?.DecorationId,
                MusicId      = source?.MusicId,
                TransporterId = source?.TransporterId ?? 0,
                Comment      = source?.Comment ?? string.Empty,
                EventTypes   = eventTypes,
                EventTimes   = eventTimes,
                PartyPlots   = MapServices(services, "Party Plot",  source?.PartyPlotId),
                Caterers     = MapServices(services, "Caterer",     source?.CatererId),
                Decorations  = MapServices(services, "Decoration",  source?.DecorationId),
                Musics       = MapServices(services, "Music",       source?.MusicId),
                Transporters = MapServices(services, "Transporter", source?.TransporterId)
            };
        }

        private static IReadOnlyList<ServiceOptionViewModel> MapServices(IEnumerable<ServiceCatalogItem> services, string category, int? selectedId)
        {
            return services
                .Where(s => s.Service_Category == category)
                .Select(s => new ServiceOptionViewModel
                {
                    Id           = s.Service_Catalog_Item_Id,
                    EventTypeId  = s.Event_Type_Id_fk,
                    Name         = s.Service_Name,
                    Category     = s.Service_Category,
                    Price        = s.Price,
                    Description  = s.Description,
                    PhotoUrl     = s.Photo_Url,
                    ProviderName = s.ServiceProvider?.Service_Provider_Name ?? string.Empty,
                    IsSelected   = s.Service_Catalog_Item_Id == selectedId
                })
                .ToList();
        }

        private void ValidateSelection(IEnumerable<ServiceCatalogItem> services, int? serviceId, string fieldName, string category, bool required)
        {
            if (!serviceId.HasValue || serviceId.Value <= 0)
            {
                if (required)
                {
                    ModelState.AddModelError(fieldName, $"Please select {category.ToLowerInvariant()} before checkout.");
                }

                return;
            }

            var isValid = services.Any(s =>
                s.Service_Catalog_Item_Id == serviceId.Value &&
                s.Service_Category == category);

            if (!isValid)
            {
                ModelState.AddModelError(fieldName, $"Please select a valid {category.ToLowerInvariant()}.");
            }
        }
    }
}
