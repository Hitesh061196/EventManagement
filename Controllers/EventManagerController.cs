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
            if (role != "Event Manager" && role != "Event Planner")
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
                Bookings = bookings,
                BookingServices = bookings.SelectMany(b => b.Services).ToList()
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmOrder(int id)
        {
            var booking = await _context.BookingCartDetails.FindAsync(id);
            if (booking != null)
            {
                booking.Event_Manager_Approved = true;
                booking.Event_Manager_Login_Id_fk = int.Parse(HttpContext.Session.GetString("UserId") ?? "0");
                booking.Booking_Status = "Confirmed by Event Manager";
                await _context.SaveChangesAsync();
                TempData["Success"] = $"Event manager confirm order completed for {booking.Booking_Reference}.";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}

