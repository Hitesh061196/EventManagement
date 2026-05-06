using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EventManagement.Models;
using EventManagement.ViewModels;

namespace EventManagement.Controllers
{
    public class EventManagerController : Controller
    {
        private readonly EventManagementDbContext _context;

        public EventManagerController(EventManagementDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var role = HttpContext.Session.GetString("Role");
            if (!AppConstants.Roles.StaffRoles.Contains(role))
            {
                return RedirectToAction("Login", "Account");
            }

            var bookings = await _context.BookingCartDetails
                .Include(b => b.Customer)
                .Include(b => b.EventType)
                .Include(b => b.Services).ThenInclude(s => s.ServiceCatalogItem)!.ThenInclude(s => s!.ServiceProvider)
                .Include(b => b.StatusUpdates)
                .OrderByDescending(b => b.Date)
                .ToListAsync();

            var model = new EventManagerDashboardViewModel
            {
                Bookings        = bookings,
                BookingServices = bookings.SelectMany(b => b.Services).ToList()
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmOrder(int id)
        {
            if (HttpContext.Session.GetString("Role") != AppConstants.Roles.EventManager)
            {
                return RedirectToAction(nameof(Index));
            }

            var booking = await _context.BookingCartDetails.FindAsync(id);
            if (booking != null)
            {
                booking.Event_Manager_Approved    = true;
                booking.Event_Manager_Login_Id_fk = int.Parse(HttpContext.Session.GetString("UserId") ?? "0");
                booking.Booking_Status            = AppConstants.BookingStatus.ConfirmedByManager;
                await _context.SaveChangesAsync();
                TempData["Success"] = $"Order {booking.Booking_Reference} has been confirmed.";
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RejectOrder(int id, string reason)
        {
            if (HttpContext.Session.GetString("Role") != AppConstants.Roles.EventManager)
            {
                return RedirectToAction(nameof(Index));
            }

            var booking = await _context.BookingCartDetails.FindAsync(id);
            if (booking != null)
            {
                booking.Event_Manager_Approved    = false;
                booking.Event_Manager_Login_Id_fk = int.Parse(HttpContext.Session.GetString("UserId") ?? "0");
                booking.Booking_Status            = AppConstants.BookingStatus.RejectedByManager;
                booking.Rejection_Reason          = reason;
                await _context.SaveChangesAsync();
                TempData["Error"] = $"Order {booking.Booking_Reference} has been rejected.";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
