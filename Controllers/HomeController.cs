using System.Diagnostics;
using EventManagement.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EventManagement.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly EventManagementDbContext _context;

        public HomeController(ILogger<HomeController> logger, EventManagementDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            ViewBag.EventTypes = await _context.EventTypeMasters
                .Where(e => e.Event_Type_Status == "Active")
                .OrderBy(e => e.Event_Type)
                .ToListAsync();

            ViewBag.FeaturedServices = await _context.ServiceCatalogItems
                .Include(s => s.EventType)
                .Include(s => s.ServiceProvider)
                .Where(s => s.Is_Active)
                .OrderBy(s => s.Service_Category)
                .Take(6)
                .ToListAsync();

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult About()
        {
            return View();
        }

        public IActionResult Contact()
        {
            return View(new InquiryDetail { Date = DateTime.Now, Inquiry_Status = true });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Contact(InquiryDetail inquiry)
        {
            if (!ModelState.IsValid)
            {
                return View(inquiry);
            }

            inquiry.Date = DateTime.Now;
            inquiry.Inquiry_Status = true;
            inquiry.Response = "Awaiting admin response";
            inquiry.Response_Date = DateTime.Now;

            _context.InquiryDetails.Add(inquiry);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Data successfully submitted.";
            return RedirectToAction(nameof(Contact));
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
