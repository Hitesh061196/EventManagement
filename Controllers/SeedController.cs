using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EventManagement.Data;
using EventManagement.Models;

namespace EventManagement.Controllers
{
    public class SeedController : Controller
    {
        private readonly EventManagementDbContext _context;
        private readonly ILogger<SeedController> _logger;

        public SeedController(EventManagementDbContext context, ILogger<SeedController> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                await EventManagementSeeder.SeedAsync(_context);
                TempData["Success"] = "Seed data applied. Admin: admin@event.com / admin123 | Event Manager: manager@event.com / manager123 | Service Provider: partyplot@event.com / partyplot123 | Customer: customer@event.com / customer123";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Seeding failed: {Message}", ex.Message);
                TempData["Error"] = $"Seeding failed: {ex.Message}";
            }
            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> Reset()
        {
            try
            {
                await DeleteAllDataAsync();
                await EventManagementSeeder.SeedAsync(_context);
                TempData["Success"] = "Database reset and re-seeded. Admin: admin@event.com / admin123 | Manager: manager@event.com / manager123 | Provider: partyplot@event.com / partyplot123 | Customer: customer@event.com / customer123";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Reset failed: {Message}", ex.Message);
                TempData["Error"] = $"Reset failed: {ex.Message}";
            }
            return RedirectToAction("Index", "Home");
        }

        private async Task DeleteAllDataAsync()
        {
            // Delete in FK dependency order (children before parents)
            await _context.Database.ExecuteSqlRawAsync("DELETE FROM [Event_Status_Master]");
            await _context.Database.ExecuteSqlRawAsync("DELETE FROM [Payment_Detail]");
            await _context.Database.ExecuteSqlRawAsync("DELETE FROM [Booking_Service_Detail]");
            await _context.Database.ExecuteSqlRawAsync("DELETE FROM [Booking_Cart_Detail]");
            await _context.Database.ExecuteSqlRawAsync("DELETE FROM [Service_Catalog_Item]");
            await _context.Database.ExecuteSqlRawAsync("DELETE FROM [FeedBack_Detail]");
            await _context.Database.ExecuteSqlRawAsync("DELETE FROM [Inquiry_Detail]");
            await _context.Database.ExecuteSqlRawAsync("DELETE FROM [Service_Provider]");
            await _context.Database.ExecuteSqlRawAsync("DELETE FROM [User_Registration_Detail]");
            await _context.Database.ExecuteSqlRawAsync("DELETE FROM [Login_Detail]");
            await _context.Database.ExecuteSqlRawAsync("DELETE FROM [Area_Master]");
            await _context.Database.ExecuteSqlRawAsync("DELETE FROM [City_Master]");
            await _context.Database.ExecuteSqlRawAsync("DELETE FROM [State_Master]");
            await _context.Database.ExecuteSqlRawAsync("DELETE FROM [Event_Type_Master]");
            await _context.Database.ExecuteSqlRawAsync("DELETE FROM [Event_Time_Master]");
            await _context.Database.ExecuteSqlRawAsync("DELETE FROM [Service_Provider_Type_Master]");
            await _context.Database.ExecuteSqlRawAsync("DELETE FROM [Roll_Detail]");
        }
    }
}
