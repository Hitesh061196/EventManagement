using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EventManagement.Models;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using EventManagement.ViewModels;

namespace EventManagement.Controllers
{
    public class ServiceProviderController : Controller
    {
        private readonly EventManagementDbContext _context;
        private readonly IWebHostEnvironment _env;

        public ServiceProviderController(EventManagementDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        public async Task<IActionResult> Index()
        {
            if (!IsProvider())
            {
                return RedirectToAction("Login", "Account");
            }

            var provider = await GetProviderAsync();
            if (provider == null)
            {
                TempData["Error"] = "Provider profile not found.";
                return RedirectToAction("Login", "Account");
            }

            var services = await _context.ServiceCatalogItems
                .Where(s => s.Service_Provider_Id_fk == provider.Service_Provider_Id)
                .OrderBy(s => s.Service_Category)
                .ThenBy(s => s.Service_Name)
                .ToListAsync();

            var orders = await _context.BookingServiceDetails
                .Include(b => b.Booking)!.ThenInclude(b => b!.Customer)
                .Include(b => b.Booking)!.ThenInclude(b => b!.EventType)
                .Include(b => b.ServiceCatalogItem)
                .Where(b => b.ServiceCatalogItem != null && b.ServiceCatalogItem.Service_Provider_Id_fk == provider.Service_Provider_Id)
                .OrderByDescending(b => b.Booking!.Date)
                .ToListAsync();

            var model = new ServiceProviderDashboardViewModel
            {
                Services = services,
                Orders = orders
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmOrder(int id, string providerComment)
        {
            var order = await _context.BookingServiceDetails
                .Include(b => b.ServiceCatalogItem)
                .FirstOrDefaultAsync(b => b.Booking_Service_Detail_Id == id);

            if (order != null)
            {
                order.Confirmation_Status = "Confirmed";
                order.Provider_Comment = providerComment;
                order.Confirmed_On = DateTime.UtcNow;
                await _context.SaveChangesAsync();
                TempData["Success"] = "Service person booking order confirmed.";
            }

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> ManageServices()
        {
            if (!IsProvider())
            {
                return RedirectToAction("Login", "Account");
            }

            var provider = await GetProviderAsync();
            var model = new ManageServicesViewModel
            {
                EventTypes = await _context.EventTypeMasters.Where(e => e.Event_Type_Status == "Active").OrderBy(e => e.Event_Type).ToListAsync(),
                ExistingServices = provider == null
                    ? Array.Empty<ServiceCatalogItem>()
                    : await _context.ServiceCatalogItems.Where(s => s.Service_Provider_Id_fk == provider.Service_Provider_Id).OrderBy(s => s.Service_Name).ToListAsync()
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ManageServices(ManageServicesViewModel model)
        {
            if (!IsProvider())
            {
                return RedirectToAction("Login", "Account");
            }

            var provider = await GetProviderAsync();
            if (provider == null)
            {
                return RedirectToAction(nameof(Index));
            }

            if (!ModelState.IsValid)
            {
                model.EventTypes = await _context.EventTypeMasters.Where(e => e.Event_Type_Status == "Active").OrderBy(e => e.Event_Type).ToListAsync();
                model.ExistingServices = await _context.ServiceCatalogItems.Where(s => s.Service_Provider_Id_fk == provider.Service_Provider_Id).OrderBy(s => s.Service_Name).ToListAsync();
                return View(model);
            }

            _context.ServiceCatalogItems.Add(new ServiceCatalogItem
            {
                Service_Name = model.ServiceName,
                Service_Category = model.ServiceCategory,
                Event_Type_Id_fk = model.EventTypeId,
                Service_Provider_Id_fk = provider.Service_Provider_Id,
                Price = model.Price,
                Photo_Url = model.PhotoUrl,
                Description = model.Description,
                Is_Active = true
            });
            await _context.SaveChangesAsync();
            TempData["Success"] = "Service person manage on services completed.";
            return RedirectToAction(nameof(ManageServices));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus(int bookingServiceId, string stage, bool status, IFormFile? photo, string description)
        {
            var bookingService = await _context.BookingServiceDetails
                .Include(b => b.ServiceCatalogItem)
                .FirstOrDefaultAsync(b => b.Booking_Service_Detail_Id == bookingServiceId);

            if (bookingService == null || bookingService.ServiceCatalogItem == null)
            {
                return RedirectToAction(nameof(Index));
            }

            var photoPath = bookingService.ServiceCatalogItem.Photo_Url;
            if (photo != null && photo.Length > 0)
            {
                var uploads = Path.Combine(_env.WebRootPath, "uploads");
                Directory.CreateDirectory(uploads);
                var fileName = $"{Guid.NewGuid():N}{Path.GetExtension(photo.FileName)}";
                var filePath = Path.Combine(uploads, fileName);
                await using var stream = new FileStream(filePath, FileMode.Create);
                await photo.CopyToAsync(stream);
                photoPath = "/uploads/" + fileName;
            }

            _context.EventStatusMasters.Add(new EventStatusMaster
            {
                Service_Person_Id_fk = int.Parse(HttpContext.Session.GetString("UserId") ?? "0"),
                Event_Booking_Cart_fk = bookingService.Booking_Id_fk,
                Event_Type_Id_fk = bookingService.ServiceCatalogItem.Event_Type_Id_fk,
                Status = status,
                Date = DateTime.UtcNow,
                Description = $"{stage}: {description}",
                Photo = photoPath,
                Event_Type = bookingService.Service_Category
            });
            await _context.SaveChangesAsync();
            TempData["Success"] = "The working condition status has been updated.";
            return RedirectToAction(nameof(Index));
        }

        private bool IsProvider()
        {
            return HttpContext.Session.GetString("Role") == "Service Provider";
        }

        private async Task<EventManagement.Models.ServiceProvider?> GetProviderAsync()
        {
            var loginId = int.Parse(HttpContext.Session.GetString("UserId") ?? "0");
            return await _context.ServiceProviders
                .Include(s => s.Area)!.ThenInclude(a => a!.City)
                .FirstOrDefaultAsync(s => s.Login_Id_fk == loginId);
        }
    }
}

