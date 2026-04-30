using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EventManagement.Models;
using EventManagement.ViewModels;

namespace EventManagement.Controllers
{
    public class AdminController : Controller
    {
        private readonly EventManagementDbContext _context;

        public AdminController(EventManagementDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            if (!IsAdmin())
            {
                return RedirectToAction("Login", "Account");
            }

            var model = new AdminDashboardViewModel
            {
                PendingProviders = await _context.ServiceProviders.CountAsync(s => s.Approval_Status == "Pending"),
                ActiveProviders = await _context.ServiceProviders.CountAsync(s => s.Approval_Status == "Approved" && !s.Is_Blocked),
                PendingBookings = await _context.BookingCartDetails.CountAsync(b => !b.Event_Manager_Approved),
                FeedbackCount = await _context.FeedBackDetails.CountAsync(),
                InquiryCount = await _context.InquiryDetails.CountAsync(),
                EventManagerCount = await _context.UserRegistrationDetails
                    .Include(u => u.Login)
                    .ThenInclude(l => l!.Roll)
                    .CountAsync(u => u.Login != null && (u.Login.Roll!.Roll_Name == "Event Manager" || u.Login.Roll.Roll_Name == "Event Planner"))
            };

            return View(model);
        }

        public async Task<IActionResult> ManageState()
        {
            if (!IsAdmin())
            {
                return RedirectToAction("Login", "Account");
            }

            return View(await _context.StateMasters.OrderBy(s => s.State_Name).ToListAsync());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddState(string stateName)
        {
            if (!_context.StateMasters.Any(s => s.State_Name == stateName))
            {
                _context.StateMasters.Add(new StateMaster { State_Name = stateName, State_Status = true });
                await _context.SaveChangesAsync();
                TempData["Success"] = "State added successfully.";
            }

            return RedirectToAction(nameof(ManageState));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditState(int id, string stateName)
        {
            var state = await _context.StateMasters.FindAsync(id);
            if (state != null)
            {
                state.State_Name = stateName;
                await _context.SaveChangesAsync();
                TempData["Success"] = "State updated successfully.";
            }

            return RedirectToAction(nameof(ManageState));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleState(int id)
        {
            var state = await _context.StateMasters.FindAsync(id);
            if (state != null)
            {
                state.State_Status = !state.State_Status;
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(ManageState));
        }

        public async Task<IActionResult> ManageCity()
        {
            if (!IsAdmin())
            {
                return RedirectToAction("Login", "Account");
            }

            ViewBag.States = await _context.StateMasters.Where(s => s.State_Status).OrderBy(s => s.State_Name).ToListAsync();
            var cities = await _context.CityMasters.Include(c => c.State).OrderBy(c => c.City_Name).ToListAsync();
            return View(cities);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddCity(string cityName, int stateId)
        {
            _context.CityMasters.Add(new CityMaster { City_Name = cityName, City_Status = true, State_Id_fk = stateId });
            await _context.SaveChangesAsync();
            TempData["Success"] = "City added successfully.";
            return RedirectToAction(nameof(ManageCity));
        }

        public async Task<IActionResult> ManageArea()
        {
            if (!IsAdmin())
            {
                return RedirectToAction("Login", "Account");
            }

            ViewBag.Cities = await _context.CityMasters.Include(c => c.State).Where(c => c.City_Status).OrderBy(c => c.City_Name).ToListAsync();
            var areas = await _context.AreaMasters.Include(a => a.City).ThenInclude(c => c!.State).OrderBy(a => a.Area_Name).ToListAsync();
            return View(areas);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddArea(string areaName, int cityId)
        {
            _context.AreaMasters.Add(new AreaMaster { Area_Name = areaName, Area_Status = true, City_Id_fk = cityId });
            await _context.SaveChangesAsync();
            TempData["Success"] = "Area added successfully.";
            return RedirectToAction(nameof(ManageArea));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleArea(int id)
        {
            var area = await _context.AreaMasters.FindAsync(id);
            if (area != null)
            {
                area.Area_Status = !area.Area_Status;
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(ManageArea));
        }

        public async Task<IActionResult> ManageEventType()
        {
            if (!IsAdmin())
            {
                return RedirectToAction("Login", "Account");
            }

            return View(await _context.EventTypeMasters.OrderBy(e => e.Event_Type).ToListAsync());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddEventType(string eventType)
        {
            _context.EventTypeMasters.Add(new EventTypeMaster { Event_Type = eventType, Event_Type_Status = "Active" });
            await _context.SaveChangesAsync();
            TempData["Success"] = "Event type added successfully.";
            return RedirectToAction(nameof(ManageEventType));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleEventType(int id)
        {
            var eventType = await _context.EventTypeMasters.FindAsync(id);
            if (eventType != null)
            {
                eventType.Event_Type_Status = eventType.Event_Type_Status == "Active" ? "Inactive" : "Active";
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(ManageEventType));
        }

        public async Task<IActionResult> ManageServiceProviders()
        {
            if (!IsAdmin())
            {
                return RedirectToAction("Login", "Account");
            }

            var providers = await _context.ServiceProviders
                .Include(s => s.Login)
                .Include(s => s.Area)
                .ThenInclude(a => a!.City)
                .ThenInclude(c => c!.State)
                .OrderBy(s => s.Service_Provider_Name)
                .ToListAsync();

            var model = new ManageServiceProvidersViewModel
            {
                PendingProviders = providers.Where(p => p.Approval_Status == "Pending").ToList(),
                ApprovedProviders = providers.Where(p => p.Approval_Status == "Approved").ToList(),
                RejectedProviders = providers.Where(p => p.Approval_Status == "Rejected").ToList()
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ApproveProvider(int id)
        {
            var provider = await _context.ServiceProviders.Include(s => s.Login).FirstOrDefaultAsync(s => s.Service_Provider_Id == id);
            if (provider?.Login != null)
            {
                provider.Approval_Status = "Approved";
                provider.Is_Blocked = false;
                provider.Rejection_Reason = string.Empty;
                provider.Login.Is_Active = true;
                provider.Login.Must_Change_Password = true;
                provider.Login.Last_Notification = $"Approval mail sent to {provider.Email_Id}: username {provider.Email_Id}, password {provider.Login.Password}";
                await _context.SaveChangesAsync();
                TempData["Success"] = "Service person approved and password sent through mail simulation.";
            }

            return RedirectToAction(nameof(ManageServiceProviders));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RejectProvider(int id, string reason)
        {
            var provider = await _context.ServiceProviders.Include(s => s.Login).FirstOrDefaultAsync(s => s.Service_Provider_Id == id);
            if (provider?.Login != null)
            {
                provider.Approval_Status = "Rejected";
                provider.Rejection_Reason = string.IsNullOrWhiteSpace(reason) ? "Does not match current onboarding requirements." : reason;
                provider.Login.Is_Active = false;
                provider.Login.Last_Notification = $"Provider rejected: {provider.Rejection_Reason}";
                await _context.SaveChangesAsync();
                TempData["Error"] = "Service person rejected.";
            }

            return RedirectToAction(nameof(ManageServiceProviders));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleProviderBlock(int id)
        {
            var provider = await _context.ServiceProviders.Include(s => s.Login).FirstOrDefaultAsync(s => s.Service_Provider_Id == id);
            if (provider?.Login != null)
            {
                provider.Is_Blocked = !provider.Is_Blocked;
                provider.Login.Is_Active = !provider.Is_Blocked && provider.Approval_Status == "Approved";
                await _context.SaveChangesAsync();
                TempData["Success"] = provider.Is_Blocked ? "Provider blocked successfully." : "Provider unblocked successfully.";
            }

            return RedirectToAction(nameof(ManageServiceProviders));
        }

        public async Task<IActionResult> ManageEventManagers()
        {
            if (!IsAdmin())
            {
                return RedirectToAction("Login", "Account");
            }

            var model = new CreateEventManagerViewModel
            {
                Areas = await _context.AreaMasters.Include(a => a.City).ThenInclude(c => c!.State).OrderBy(a => a.Area_Name).ToListAsync(),
                ExistingStaff = await _context.UserRegistrationDetails
                    .Include(u => u.Area).ThenInclude(a => a!.City).ThenInclude(c => c!.State)
                    .Include(u => u.Login).ThenInclude(l => l!.Roll)
                    .Where(u => u.Login != null && (u.Login.Roll!.Roll_Name == "Event Manager" || u.Login.Roll.Roll_Name == "Event Planner"))
                    .OrderBy(u => u.First_Name)
                    .ToListAsync()
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateEventManager(CreateEventManagerViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.Areas = await _context.AreaMasters.Include(a => a.City).ThenInclude(c => c!.State).OrderBy(a => a.Area_Name).ToListAsync();
                model.ExistingStaff = Array.Empty<UserRegistrationDetail>();
                return View("ManageEventManagers", model);
            }

            if (await _context.LoginDetails.AnyAsync(l => l.User_Name == model.Email))
            {
                TempData["Error"] = "This email is already used by another account.";
                return RedirectToAction(nameof(ManageEventManagers));
            }

            var roleId = await _context.RollDetails.Where(r => r.Roll_Name == model.RoleName).Select(r => r.Roll_Id).FirstAsync();
            var generatedPassword = model.RoleName == "Event Planner" ? "planner123" : "manager123";

            var login = new LoginDetail
            {
                User_Name = model.Email,
                Password = generatedPassword,
                Roll_Id_fk = roleId,
                Is_Active = true,
                Must_Change_Password = true,
                Last_Notification = $"Credentials sent to {model.Email}: username {model.Email}, password {generatedPassword}"
            };
            _context.LoginDetails.Add(login);
            await _context.SaveChangesAsync();

            _context.UserRegistrationDetails.Add(new UserRegistrationDetail
            {
                First_Name = model.FirstName,
                Last_Name = model.LastName,
                Address = "Head office assignment",
                Contact_No = model.ContactNo,
                Email_Id = model.Email,
                Gender = "Other",
                Area_Id_fk = model.AreaId,
                Login_Id_fk = login.Login_Id
            });
            await _context.SaveChangesAsync();

            TempData["Success"] = $"{model.RoleName} created. Mail simulation prepared with username and password.";
            return RedirectToAction(nameof(ManageEventManagers));
        }

        public async Task<IActionResult> ViewFeedback()
        {
            if (!IsAdmin())
            {
                return RedirectToAction("Login", "Account");
            }

            return View(await _context.FeedBackDetails.OrderByDescending(f => f.Created_On).ToListAsync());
        }

        public async Task<IActionResult> ViewInquiry()
        {
            if (!IsAdmin())
            {
                return RedirectToAction("Login", "Account");
            }

            return View(await _context.InquiryDetails.OrderByDescending(i => i.Date).ToListAsync());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RespondInquiry(int id, string response)
        {
            var inquiry = await _context.InquiryDetails.FindAsync(id);
            if (inquiry != null)
            {
                inquiry.Response = response;
                inquiry.Response_Date = DateTime.Now;
                await _context.SaveChangesAsync();
                TempData["Success"] = "Response sent through mail simulation.";
            }

            return RedirectToAction(nameof(ViewInquiry));
        }

        private bool IsAdmin()
        {
            return HttpContext.Session.GetString("Role") == "Admin";
        }
    }
}

