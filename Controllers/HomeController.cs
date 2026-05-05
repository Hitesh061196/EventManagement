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
                .Where(e => e.Event_Type_Status == AppConstants.EventTypeStatus.Active)
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

        public async Task<IActionResult> About()
        {
            ViewBag.EventTypeCount    = await _context.EventTypeMasters.CountAsync(e => e.Event_Type_Status == AppConstants.EventTypeStatus.Active);
            ViewBag.ProviderCount     = await _context.ServiceProviders.CountAsync(s => s.Approval_Status == AppConstants.ApprovalStatus.Approved && !s.Is_Blocked);
            ViewBag.ServiceCount      = await _context.ServiceCatalogItems.CountAsync(s => s.Is_Active);
            ViewBag.BookingCount      = await _context.BookingCartDetails.CountAsync();
            ViewBag.ActiveEventTypes  = await _context.EventTypeMasters
                .Where(e => e.Event_Type_Status == AppConstants.EventTypeStatus.Active)
                .OrderBy(e => e.Event_Type)
                .ToListAsync();
            ViewBag.ServiceTypes      = await _context.ServiceProviderTypeMasters
                .Where(t => t.Is_Active)
                .OrderBy(t => t.Provider_Type_Id)
                .ToListAsync();
            return View();
        }

        public async Task<IActionResult> Contact()
        {
            var inquiry = new InquiryDetail { Date = DateTime.Now, Inquiry_Status = true };

            var loginIdStr = HttpContext.Session.GetString("UserId");
            if (int.TryParse(loginIdStr, out var loginId))
            {
                var email = HttpContext.Session.GetString("UserName") ?? string.Empty;
                inquiry.Email = email;

                var role = HttpContext.Session.GetString("Role");
                if (role == AppConstants.Roles.ServiceProvider)
                {
                    var sp = await _context.ServiceProviders.FirstOrDefaultAsync(s => s.Login_Id_fk == loginId);
                    if (sp != null) inquiry.Name = sp.Service_Provider_Name;
                }
                else
                {
                    var user = await _context.UserRegistrationDetails.FirstOrDefaultAsync(u => u.Login_Id_fk == loginId);
                    if (user != null) inquiry.Name = $"{user.First_Name} {user.Last_Name}".Trim();
                }
            }

            return View(inquiry);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Contact(InquiryDetail inquiry)
        {
            if (!ModelState.IsValid)
            {
                return View(inquiry);
            }

            inquiry.Date           = DateTime.Now;
            inquiry.Inquiry_Status = true;
            inquiry.Response       = "Awaiting admin response";
            inquiry.Response_Date  = DateTime.Now;

            _context.InquiryDetails.Add(inquiry);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Your inquiry has been submitted successfully! We will get back to you soon.";
            return RedirectToAction(nameof(Contact));
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
