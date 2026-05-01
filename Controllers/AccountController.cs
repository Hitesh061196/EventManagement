using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EventManagement.Models;

namespace EventManagement.Controllers
{
    public class AccountController : Controller
    {
        private readonly EventManagementDbContext _context;

        public AccountController(EventManagementDbContext context)
        {
            _context = context;
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string userName, string password)
        {
            var login = await _context.LoginDetails
                .Include(l => l.Roll)
                .FirstOrDefaultAsync(l => l.User_Name == userName && l.Password == password);

            if (login != null)
            {
                if (!login.Is_Active)
                {
                    TempData["Error"] = "Your account is waiting for admin approval or has been blocked.";
                    return View();
                }

                HttpContext.Session.SetString("UserId", login.Login_Id.ToString());
                HttpContext.Session.SetString("UserName", login.User_Name);
                HttpContext.Session.SetString("Role", login.Roll!.Roll_Name);
                return login.Roll.Roll_Name switch
                {
                    "Admin" => RedirectToAction("Index", "Admin"),
                    "Customer" => RedirectToAction("BookEvent", "Customer"),
                    "Service Provider" => RedirectToAction("Index", "ServiceProvider"),
                    "Event Manager" or "Event Planner" => RedirectToAction("Index", "EventManager"),
                    _ => RedirectToAction("Index", "Home")
                };
            }

            TempData["Error"] = "Invalid credentials";
            return View();
        }

        public async Task<IActionResult> Register()
        {
            ViewBag.States = await _context.StateMasters.Where(s => s.State_Status).OrderBy(s => s.State_Name).ToListAsync();
            return View();
        }

        public async Task<IActionResult> GetCitiesByState(int stateId)
        {
            var cities = await _context.CityMasters
                .Where(c => c.State_Id_fk == stateId && c.City_Status)
                .OrderBy(c => c.City_Name)
                .Select(c => new { c.City_Id, c.City_Name })
                .ToListAsync();
            return Json(cities);
        }

        public async Task<IActionResult> GetAreasByCity(int cityId)
        {
            var areas = await _context.AreaMasters
                .Where(a => a.City_Id_fk == cityId && a.Area_Status)
                .OrderBy(a => a.Area_Name)
                .Select(a => new { a.Area_Id, a.Area_Name })
                .ToListAsync();
            return Json(areas);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(
            string firstName,
            string lastName,
            string address,
            string contactNo,
            string email,
            string gender,
            int areaId,
            string password,
            string confirmPassword)
        {
            if (password != confirmPassword)
            {
                TempData["Error"] = "Password and confirm password do not match.";
                return RedirectToAction(nameof(Register));
            }

            if (await _context.LoginDetails.AnyAsync(l => l.User_Name == email) ||
                await _context.UserRegistrationDetails.AnyAsync(u => u.Email_Id == email))
            {
                TempData["Error"] = "User registration email id is already registered.";
                return RedirectToAction(nameof(Register));
            }

            var customerRoleId = await _context.RollDetails
                .Where(r => r.Roll_Name == "Customer")
                .Select(r => r.Roll_Id)
                .FirstAsync();

            var login = new LoginDetail
            {
                User_Name = email,
                Password = password,
                Roll_Id_fk = customerRoleId,
                Is_Active = true
            };
            _context.LoginDetails.Add(login);
            await _context.SaveChangesAsync();

            _context.UserRegistrationDetails.Add(new UserRegistrationDetail
            {
                First_Name = firstName,
                Last_Name = lastName,
                Address = address,
                Contact_No = contactNo,
                Email_Id = email,
                Gender = gender,
                Area_Id_fk = areaId,
                Login_Id_fk = login.Login_Id
            });
            await _context.SaveChangesAsync();

            TempData["Success"] = "User registration successfully completed. Please login.";
            return RedirectToAction(nameof(Login));
        }

        public async Task<IActionResult> ServiceProviderRegister()
        {
            ViewBag.States = await _context.StateMasters.Where(s => s.State_Status).OrderBy(s => s.State_Name).ToListAsync();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ServiceProviderRegister(
            string providerName,
            string email,
            string mobileNo,
            string shopName,
            string gender,
            string address,
            int areaId,
            string serviceType,
            string description,
            string profileImageUrl)
        {
            if (await _context.ServiceProviders.AnyAsync(s => s.Email_Id == email) ||
                await _context.LoginDetails.AnyAsync(l => l.User_Name == email))
            {
                TempData["Error"] = "This service person email is already registered.";
                return RedirectToAction(nameof(ServiceProviderRegister));
            }

            var roleId = await _context.RollDetails.Where(r => r.Roll_Name == "Service Provider").Select(r => r.Roll_Id).FirstAsync();
            var generatedPassword = $"{serviceType.Replace(" ", string.Empty).ToLowerInvariant()}123";

            var login = new LoginDetail
            {
                User_Name = email,
                Password = generatedPassword,
                Roll_Id_fk = roleId,
                Is_Active = false,
                Must_Change_Password = true,
                Last_Notification = $"Pending approval for {providerName}. Proposed credentials {email}/{generatedPassword}."
            };
            _context.LoginDetails.Add(login);
            await _context.SaveChangesAsync();

            _context.ServiceProviders.Add(new EventManagement.Models.ServiceProvider
            {
                Service_Provider_Name = providerName,
                Email_Id = email,
                Mobile_No = mobileNo,
                Shop_Name = shopName,
                Gender = gender,
                Address = address,
                Service_Provider_Area_Id_fk = areaId,
                Service_Provider_Type = serviceType,
                Login_Id_fk = login.Login_Id,
                Approval_Status = "Pending",
                Description = description,
                Profile_Image_Url = profileImageUrl
            });
            await _context.SaveChangesAsync();

            TempData["Success"] = "Service person added successfully. Admin approval is pending.";
            return RedirectToAction(nameof(Login));
        }

        public IActionResult ChangePassword()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("UserId")))
            {
                return RedirectToAction(nameof(Login));
            }

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(string currentPassword, string newPassword, string confirmPassword)
        {
            var loginIdString = HttpContext.Session.GetString("UserId");
            if (!int.TryParse(loginIdString, out var loginId))
            {
                return RedirectToAction(nameof(Login));
            }

            var login = await _context.LoginDetails.FindAsync(loginId);
            if (login == null || login.Password != currentPassword)
            {
                TempData["Error"] = "Current password is invalid.";
                return View();
            }

            if (newPassword != confirmPassword)
            {
                TempData["Error"] = "New password and confirm password must match.";
                return View();
            }

            login.Password = newPassword;
            login.Must_Change_Password = false;
            await _context.SaveChangesAsync();
            TempData["Success"] = "Password changed successfully.";
            return RedirectToAction(nameof(Login));
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }
    }
}

