using Microsoft.EntityFrameworkCore;
using EventManagement.Models;
using EventManagement.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddSession();
builder.Services.AddDbContext<EventManagementDbContext>(options =>
    options
        .UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
        .ConfigureWarnings(w => w.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.RelationalEventId.PendingModelChangesWarning)));

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<EventManagementDbContext>();
    var logger    = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
    try
    {
        await EventManagementSeeder.SeedAsync(dbContext);
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Database seeding failed: {Message}", ex.Message);
        throw;
    }
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseSession();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
