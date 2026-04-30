using Microsoft.AspNetCore.Mvc;
using EventManagement.Data;
using EventManagement.Models;

namespace EventManagement.Controllers
{
    public class SeedController : Controller
    {
        private readonly EventManagementDbContext _context;

        public SeedController(EventManagementDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            EventManagementSeeder.SeedAsync(_context).GetAwaiter().GetResult();
            TempData["Success"] = "Seed data added. Admin: admin/admin123, Customer: customer@event.com/customer123, Event Manager: manager@event.com/manager123";
            return RedirectToAction("Index", "Home");
        }
    }
}

