using EventManagement.Models;
using Microsoft.EntityFrameworkCore;

namespace EventManagement.Data
{
    public static class EventManagementSeeder
    {
        public static async Task SeedAsync(EventManagementDbContext context)
        {
            await context.Database.MigrateAsync();

            await SeedRolesAsync(context);
            await SeedGeographyAsync(context);
            await SeedEventTypesAsync(context);
            await SeedEventTimesAsync(context);
            await SeedServiceProviderTypesAsync(context);

            var roles = await context.RollDetails.ToDictionaryAsync(r => r.Roll_Name, r => r.Roll_Id);
            var areas = await context.AreaMasters.OrderBy(a => a.Area_Id).ToListAsync();
            var eventTypes = await context.EventTypeMasters.OrderBy(e => e.Event_Id).ToListAsync();

            if (areas.Count < 4)
                throw new InvalidOperationException(
                    $"Geographic seed data is incomplete: only {areas.Count} area(s) found. " +
                    "Check State_Master, City_Master, and Area_Master tables.");

            await SeedAdminAsync(context, roles);
            await SeedStaffAsync(context, roles, areas);
            await SeedServiceProvidersAsync(context, roles, areas);
            await SeedServiceCatalogAsync(context, eventTypes);
            await SeedInquiryAsync(context);
            await SeedFeedbackAsync(context);
            await SeedSampleBookingAsync(context);
        }

        // ── Roles ─────────────────────────────────────────────────────────────────

        private static async Task SeedRolesAsync(EventManagementDbContext context)
        {
            if (await context.RollDetails.AnyAsync()) return;

            context.RollDetails.AddRange(
                new RollDetail { Roll_Name = "Admin",            Status = true },
                new RollDetail { Roll_Name = "Customer",         Status = true },
                new RollDetail { Roll_Name = "Event Manager",    Status = true },
                new RollDetail { Roll_Name = "Event Planner",    Status = true },
                new RollDetail { Roll_Name = "Service Provider", Status = true });
            await context.SaveChangesAsync();
        }

        // ── Geography — each level has its own idempotent guard ───────────────────

        private static async Task SeedGeographyAsync(EventManagementDbContext context)
        {
            if (!await context.StateMasters.AnyAsync())
            {
                context.StateMasters.AddRange(
                    new StateMaster { State_Name = "Gujarat",     State_Status = true },
                    new StateMaster { State_Name = "Maharashtra", State_Status = true });
                await context.SaveChangesAsync();
            }

            if (!await context.CityMasters.AnyAsync())
            {
                var gujarat     = await context.StateMasters.FirstAsync(s => s.State_Name == "Gujarat");
                var maharashtra = await context.StateMasters.FirstAsync(s => s.State_Name == "Maharashtra");

                context.CityMasters.AddRange(
                    new CityMaster { City_Name = "Surat",     City_Status = true, State_Id_fk = gujarat.State_Id },
                    new CityMaster { City_Name = "Ahmedabad", City_Status = true, State_Id_fk = gujarat.State_Id },
                    new CityMaster { City_Name = "Mumbai",    City_Status = true, State_Id_fk = maharashtra.State_Id });
                await context.SaveChangesAsync();
            }

            if (!await context.AreaMasters.AnyAsync())
            {
                var surat     = await context.CityMasters.FirstAsync(c => c.City_Name == "Surat");
                var ahmedabad = await context.CityMasters.FirstAsync(c => c.City_Name == "Ahmedabad");
                var mumbai    = await context.CityMasters.FirstAsync(c => c.City_Name == "Mumbai");

                context.AreaMasters.AddRange(
                    new AreaMaster { Area_Name = "Vesu",      Area_Status = true, City_Id_fk = surat.City_Id },
                    new AreaMaster { Area_Name = "Adajan",    Area_Status = true, City_Id_fk = surat.City_Id },
                    new AreaMaster { Area_Name = "Satellite", Area_Status = true, City_Id_fk = ahmedabad.City_Id },
                    new AreaMaster { Area_Name = "Andheri",   Area_Status = true, City_Id_fk = mumbai.City_Id });
                await context.SaveChangesAsync();
            }
        }

        // ── Event Types ───────────────────────────────────────────────────────────

        private static async Task SeedEventTypesAsync(EventManagementDbContext context)
        {
            if (await context.EventTypeMasters.AnyAsync()) return;

            context.EventTypeMasters.AddRange(
                new EventTypeMaster { Event_Type = "Wedding",   Event_Type_Status = "Active" },
                new EventTypeMaster { Event_Type = "Birthday",  Event_Type_Status = "Active" },
                new EventTypeMaster { Event_Type = "Corporate", Event_Type_Status = "Active" },
                new EventTypeMaster { Event_Type = "Reception", Event_Type_Status = "Active" });
            await context.SaveChangesAsync();
        }

        // ── Event Times ───────────────────────────────────────────────────────────

        private static async Task SeedEventTimesAsync(EventManagementDbContext context)
        {
            if (await context.EventTimeMasters.AnyAsync()) return;

            context.EventTimeMasters.AddRange(
                new EventTimeMaster { Event_Time_Name = "Dinner",    Is_Active = true },
                new EventTimeMaster { Event_Time_Name = "Lunch",     Is_Active = true },
                new EventTimeMaster { Event_Time_Name = "Breakfast", Is_Active = true },
                new EventTimeMaster { Event_Time_Name = "Evening",   Is_Active = true },
                new EventTimeMaster { Event_Time_Name = "Full Day",  Is_Active = true });
            await context.SaveChangesAsync();
        }

        // ── Service Provider Types ────────────────────────────────────────────────

        private static async Task SeedServiceProviderTypesAsync(EventManagementDbContext context)
        {
            if (await context.ServiceProviderTypeMasters.AnyAsync()) return;

            context.ServiceProviderTypeMasters.AddRange(
                new ServiceProviderTypeMaster { Type_Name = "Party Plot",   Is_Active = true },
                new ServiceProviderTypeMaster { Type_Name = "Caterer",      Is_Active = true },
                new ServiceProviderTypeMaster { Type_Name = "Decoration",   Is_Active = true },
                new ServiceProviderTypeMaster { Type_Name = "Music",        Is_Active = true },
                new ServiceProviderTypeMaster { Type_Name = "Transporter",  Is_Active = true });
            await context.SaveChangesAsync();
        }

        // ── Admin ─────────────────────────────────────────────────────────────────

        private static async Task SeedAdminAsync(EventManagementDbContext context, Dictionary<string, int> roles)
        {
            // Migrate legacy "admin" username to email format
            var legacy = await context.LoginDetails.FirstOrDefaultAsync(l => l.User_Name == "admin");
            if (legacy != null)
            {
                legacy.User_Name = "admin@event.com";
                await context.SaveChangesAsync();
            }

            if (await context.LoginDetails.AnyAsync(l => l.User_Name == "admin@event.com")) return;

            context.LoginDetails.Add(new LoginDetail
            {
                User_Name    = "admin@event.com",
                Password     = "admin123",
                Roll_Id_fk   = roles["Admin"],
                Is_Active    = true
            });
            await context.SaveChangesAsync();
        }

        // ── Staff (Event Manager, Event Planner, Customer) ────────────────────────

        private static async Task SeedStaffAsync(
            EventManagementDbContext context,
            Dictionary<string, int> roles,
            IReadOnlyList<AreaMaster> areas)
        {
            await EnsureStaffAsync(context, roles["Event Manager"], areas[0].Area_Id,
                "manager@event.com",  "manager123",  "Mihir", "Manager");
            await EnsureStaffAsync(context, roles["Event Planner"], areas[1].Area_Id,
                "planner@event.com",  "planner123",  "Pooja", "Planner");
            await EnsureCustomerAsync(context, roles["Customer"], areas[2].Area_Id,
                "customer@event.com", "customer123", "Riya",  "Patel");
        }

        private static async Task EnsureStaffAsync(
            EventManagementDbContext context, int roleId, int areaId,
            string email, string password, string firstName, string lastName)
        {
            if (await context.LoginDetails.AnyAsync(l => l.User_Name == email)) return;

            var login = new LoginDetail
            {
                User_Name             = email,
                Password              = password,
                Roll_Id_fk            = roleId,
                Is_Active             = true,
                Must_Change_Password  = true,
                Last_Notification     = $"Credentials sent to {email}: {email} / {password}"
            };
            context.LoginDetails.Add(login);
            await context.SaveChangesAsync();

            context.UserRegistrationDetails.Add(new UserRegistrationDetail
            {
                First_Name   = firstName,
                Middle_Name  = string.Empty,
                Last_Name    = lastName,
                Address      = "Event Management Office",
                Contact_No   = "9999999999",
                Email_Id     = email,
                Gender       = "Other",
                Area_Id_fk   = areaId,
                Login_Id_fk  = login.Login_Id
            });
            await context.SaveChangesAsync();
        }

        private static async Task EnsureCustomerAsync(
            EventManagementDbContext context, int roleId, int areaId,
            string email, string password, string firstName, string lastName)
        {
            if (await context.LoginDetails.AnyAsync(l => l.User_Name == email)) return;

            var login = new LoginDetail
            {
                User_Name    = email,
                Password     = password,
                Roll_Id_fk   = roleId,
                Is_Active    = true
            };
            context.LoginDetails.Add(login);
            await context.SaveChangesAsync();

            context.UserRegistrationDetails.Add(new UserRegistrationDetail
            {
                First_Name   = firstName,
                Middle_Name  = string.Empty,
                Last_Name    = lastName,
                Address      = "Shyam Residency, Satellite",
                Contact_No   = "9876543210",
                Email_Id     = email,
                Gender       = "Female",
                Area_Id_fk   = areaId,
                Login_Id_fk  = login.Login_Id
            });
            await context.SaveChangesAsync();
        }

        // ── Service Providers ─────────────────────────────────────────────────────

        private static async Task SeedServiceProvidersAsync(
            EventManagementDbContext context,
            Dictionary<string, int> roles,
            IReadOnlyList<AreaMaster> areas)
        {
            if (await context.ServiceProviders.AnyAsync()) return;

            await CreateProviderAsync(context, roles["Service Provider"], areas[0].Area_Id,
                name: "Akib Party Plot", email: "partyplot@event.com",
                providerType: "Party Plot", shopName: "Akib Party Plot Lawns",
                mobile: "9898001001", gender: "Male", approved: true,
                description: "Premium lawn setups for weddings and receptions.",
                imageUrl: "https://images.unsplash.com/photo-1519167758481-83f550bb49b3?w=1200");

            await CreateProviderAsync(context, roles["Service Provider"], areas[0].Area_Id,
                name: "Royal Caterers", email: "caterer@event.com",
                providerType: "Caterer", shopName: "Royal Catering Services",
                mobile: "9898002002", gender: "Male", approved: true,
                description: "Multi-cuisine catering team for grand and boutique events.",
                imageUrl: "https://images.unsplash.com/photo-1555244162-803834f70033?w=1200");

            await CreateProviderAsync(context, roles["Service Provider"], areas[1].Area_Id,
                name: "Bloom Decorations", email: "decor@event.com",
                providerType: "Decoration", shopName: "Bloom Decor Studio",
                mobile: "9898003003", gender: "Female", approved: true,
                description: "Theme-based floral and stage decoration specialist.",
                imageUrl: "https://images.unsplash.com/photo-1519741497674-611481863552?w=1200");

            await CreateProviderAsync(context, roles["Service Provider"], areas[1].Area_Id,
                name: "Rhythm Beats", email: "music@event.com",
                providerType: "Music", shopName: "Rhythm Beats Entertainment",
                mobile: "9898004004", gender: "Male", approved: true,
                description: "Live DJ, dhol, and sound setup for every celebration.",
                imageUrl: "https://images.unsplash.com/photo-1493225457124-a3eb161ffa5f?w=1200");

            await CreateProviderAsync(context, roles["Service Provider"], areas[2].Area_Id,
                name: "Swift Transport", email: "transport@event.com",
                providerType: "Transporter", shopName: "Swift Event Transport Co.",
                mobile: "9898005005", gender: "Male", approved: true,
                description: "Guest pickup and VIP transport with premium vehicles.",
                imageUrl: "https://images.unsplash.com/photo-1503376780353-7e6692767b70?w=1200");

            await CreateProviderAsync(context, roles["Service Provider"], areas[3].Area_Id,
                name: "New Light Decor", email: "pendingprovider@event.com",
                providerType: "Decoration", shopName: "New Light Decor House",
                mobile: "9898006006", gender: "Female", approved: false,
                description: "Newly registered decoration vendor waiting for admin approval.",
                imageUrl: "https://images.unsplash.com/photo-1469371670807-013ccf25f16a?w=1200");
        }

        private static async Task CreateProviderAsync(
            EventManagementDbContext context, int roleId, int areaId,
            string name, string email, string providerType, string shopName,
            string mobile, string gender, bool approved,
            string description, string imageUrl)
        {
            if (await context.ServiceProviders.AnyAsync(p => p.Email_Id == email)) return;

            var password = $"{providerType.Replace(" ", string.Empty).ToLowerInvariant()}123";
            var login = new LoginDetail
            {
                User_Name            = email,
                Password             = password,
                Roll_Id_fk           = roleId,
                Is_Active            = approved,
                Must_Change_Password = approved,
                Last_Notification    = approved
                    ? $"Approval mail sent to {email}: username {email}, password {password}"
                    : $"Registration received for {name}. Approval pending."
            };
            context.LoginDetails.Add(login);
            await context.SaveChangesAsync();

            context.ServiceProviders.Add(new EventManagement.Models.ServiceProvider
            {
                Service_Provider_Name      = name,
                Email_Id                   = email,
                Mobile_No                  = mobile,
                Shop_Name                  = shopName,
                Gender                     = gender,
                Address                    = "Service Plaza",
                Service_Provider_Area_Id_fk = areaId,
                Service_Provider_Type      = providerType,
                Login_Id_fk                = login.Login_Id,
                Approval_Status            = approved ? "Approved" : "Pending",
                Is_Blocked                 = false,
                Description                = description,
                Profile_Image_Url          = imageUrl
            });
            await context.SaveChangesAsync();
        }

        // ── Service Catalog ───────────────────────────────────────────────────────

        private static async Task SeedServiceCatalogAsync(
            EventManagementDbContext context,
            IReadOnlyList<EventTypeMaster> eventTypes)
        {
            if (await context.ServiceCatalogItems.AnyAsync()) return;

            var providers = await context.ServiceProviders.Include(p => p.Login).ToListAsync();
            var wedding   = eventTypes.First(e => e.Event_Type == "Wedding").Event_Id;
            var birthday  = eventTypes.First(e => e.Event_Type == "Birthday").Event_Id;
            var corporate = eventTypes.First(e => e.Event_Type == "Corporate").Event_Id;

            context.ServiceCatalogItems.AddRange(
                MakeService("Akib Party Plot - Royal Lawn",    "Party Plot",  wedding,   providers, "Party Plot",  180000, "Per Event", "Lavish open lawn with mandap-ready setup.",                     "https://images.unsplash.com/photo-1511578314322-379afb476865?w=1200"),
                MakeService("Skyline Banquet",                 "Party Plot",  corporate, providers, "Party Plot",  135000, "Per Event", "Indoor banquet for corporate and reception events.",            "https://images.unsplash.com/photo-1517457373958-b7bdd4587205?w=1200"),
                MakeService("Royal Caterers Signature Menu",   "Caterer",     wedding,   providers, "Caterer",      95000, "Per Plate", "Gujarati, Punjabi, Italian, and dessert counters.",             "https://images.unsplash.com/photo-1528605248644-14dd04022da1?w=1200"),
                MakeService("Celebration Caterers",            "Caterer",     birthday,  providers, "Caterer",      55000, "Per Plate", "Buffet and live counters ideal for birthday events.",           "https://images.unsplash.com/photo-1414235077428-338989a2e8c0?w=1200"),
                MakeService("Bloom Floral Stage",              "Decoration",  wedding,   providers, "Decoration",   42000, "Per Event", "Pastel floral backdrop and aisle decor.",                      "https://images.unsplash.com/photo-1520854221256-17451cc331bf?w=1200"),
                MakeService("Corporate Brand Decor",           "Decoration",  corporate, providers, "Decoration",   30000, "Per Event", "LED branding walls and formal stage decor.",                   "https://images.unsplash.com/photo-1511795409834-ef04bbd61622?w=1200"),
                MakeService("Rhythm DJ Night",                 "Music",       wedding,   providers, "Music",        25000, "Per Event", "DJ, percussion, and dance floor setup.",                       "https://images.unsplash.com/photo-1501386761578-eac5c94b800a?w=1200"),
                MakeService("Kids Party Music Setup",          "Music",       birthday,  providers, "Music",        15000, "Per Event", "Light music, announcements, and effects for birthdays.",        "https://images.unsplash.com/photo-1499364615650-ec38552f4f34?w=1200"),
                MakeService("Swift Guest Shuttle",             "Transporter", wedding,   providers, "Transporter",  22000, "Per Trip",  "Guest shuttle and VIP sedan arrangements.",                    "https://images.unsplash.com/photo-1449965408869-eaa3f722e40d?w=1200"),
                MakeService("Executive Transfer Fleet",        "Transporter", corporate, providers, "Transporter",  28000, "Per Trip",  "Premium transport for conferences and business events.",        "https://images.unsplash.com/photo-1494976388531-d1058494cdd8?w=1200"));
            await context.SaveChangesAsync();
        }

        private static ServiceCatalogItem MakeService(
            string serviceName, string category, int eventTypeId,
            IReadOnlyList<EventManagement.Models.ServiceProvider> providers,
            string providerType, decimal price, string priceType,
            string description, string photoUrl)
        {
            var provider = providers.First(p => p.Service_Provider_Type == providerType && p.Approval_Status == "Approved");
            return new ServiceCatalogItem
            {
                Service_Name           = serviceName,
                Service_Category       = category,
                Event_Type_Id_fk       = eventTypeId,
                Service_Provider_Id_fk = provider.Service_Provider_Id,
                Price                  = price,
                Price_Type             = priceType,
                Description            = description,
                Photo_Url              = photoUrl,
                Is_Active              = true
            };
        }

        // ── Inquiry ───────────────────────────────────────────────────────────────

        private static async Task SeedInquiryAsync(EventManagementDbContext context)
        {
            if (await context.InquiryDetails.AnyAsync()) return;

            context.InquiryDetails.Add(new InquiryDetail
            {
                Name           = "Aman Shah",
                Email          = "aman@example.com",
                Question       = "Can you arrange a wedding package for 350 guests in Surat?",
                Inquiry_Status = true,
                Date           = DateTime.Today.AddDays(-2),
                Response       = "We have replied with package details.",
                Response_Date  = DateTime.Today.AddDays(-1)
            });
            await context.SaveChangesAsync();
        }

        // ── Feedback ──────────────────────────────────────────────────────────────

        private static async Task SeedFeedbackAsync(EventManagementDbContext context)
        {
            if (await context.FeedBackDetails.AnyAsync()) return;

            context.FeedBackDetails.Add(new FeedBackDetail
            {
                Name           = "Riya Patel",
                Email          = "customer@event.com",
                FeedBack       = "The wedding package flow is easy to use and the service cards are very clear.",
                FeedBack_Status = true,
                Created_On     = DateTime.UtcNow.AddDays(-1)
            });
            await context.SaveChangesAsync();
        }

        // ── Sample Booking ────────────────────────────────────────────────────────

        private static async Task SeedSampleBookingAsync(EventManagementDbContext context)
        {
            if (await context.BookingCartDetails.AnyAsync()) return;

            var customer = await context.UserRegistrationDetails
                .Include(u => u.Login)
                .FirstAsync(u => u.Email_Id == "customer@event.com");

            var managerLogin = await context.LoginDetails.FirstAsync(l => l.User_Name == "manager@event.com");
            var wedding      = await context.EventTypeMasters.FirstAsync(e => e.Event_Type == "Wedding");

            var partyPlot  = await context.ServiceCatalogItems.FirstAsync(s => s.Service_Category == "Party Plot");
            var caterer    = await context.ServiceCatalogItems.FirstAsync(s => s.Service_Category == "Caterer");
            var decoration = await context.ServiceCatalogItems.FirstAsync(s => s.Service_Category == "Decoration");
            var music      = await context.ServiceCatalogItems.FirstAsync(s => s.Service_Category == "Music");
            var transport  = await context.ServiceCatalogItems.FirstAsync(s => s.Service_Category == "Transporter");

            var booking = new BookingCartDetail
            {
                Event_Package_Id_fk      = wedding.Event_Id,
                Custom_Id_fk             = customer.User_Id,
                Date                     = DateTime.Today.AddDays(15),
                Guest_Count              = 250,
                Ammount                  = partyPlot.Price + caterer.Price + decoration.Price + music.Price + transport.Price,
                Status                   = true,
                Comment                  = "Sample booking created from seed data.",
                Booking_Reference        = $"EM-{DateTime.Today:yyyyMMdd}-1001",
                Booking_Status           = "Confirmed by Event Manager",
                Payment_Status           = "Pending",
                Event_Time               = "Evening",
                From_Time                = "06:00 PM",
                To_Time                  = "11:00 PM",
                Event_Manager_Approved   = true,
                Event_Manager_Login_Id_fk = managerLogin.Login_Id,
                Created_On               = DateTime.UtcNow.AddDays(-2)
            };
            context.BookingCartDetails.Add(booking);
            await context.SaveChangesAsync();

            foreach (var service in new[] { partyPlot, caterer, decoration, music, transport })
            {
                context.BookingServiceDetails.Add(new BookingServiceDetail
                {
                    Booking_Id_fk              = booking.Booking_Id,
                    Service_Catalog_Item_Id_fk = service.Service_Catalog_Item_Id,
                    Service_Category           = service.Service_Category,
                    Price                      = service.Price,
                    Confirmation_Status        = "Confirmed",
                    Provider_Comment           = $"{service.Service_Category} team confirmed the order.",
                    Confirmed_On               = DateTime.UtcNow.AddDays(-1)
                });
            }

            var providerLogins = await context.ServiceProviders
                .ToDictionaryAsync(p => p.Service_Provider_Type, p => p.Login_Id_fk);

            context.EventStatusMasters.AddRange(
                new EventStatusMaster
                {
                    Service_Person_Id_fk   = providerLogins["Party Plot"],
                    Event_Booking_Cart_fk  = booking.Booking_Id,
                    Event_Type_Id_fk       = wedding.Event_Id,
                    Status                 = true,
                    Date                   = DateTime.UtcNow.AddDays(-1),
                    Photo                  = partyPlot.Photo_Url,
                    Description            = "Collect Material stage completed.",
                    Event_Type             = "Party Plot"
                },
                new EventStatusMaster
                {
                    Service_Person_Id_fk   = providerLogins["Decoration"],
                    Event_Booking_Cart_fk  = booking.Booking_Id,
                    Event_Type_Id_fk       = wedding.Event_Id,
                    Status                 = true,
                    Date                   = DateTime.UtcNow,
                    Photo                  = decoration.Photo_Url,
                    Description            = "Stage decoration completed and photos uploaded.",
                    Event_Type             = "Decoration"
                });

            await context.SaveChangesAsync();
        }
    }
}
