using Microsoft.EntityFrameworkCore;
using EventManagement.Models;

namespace EventManagement.Models
{
    public class EventManagementDbContext : DbContext
    {
        public EventManagementDbContext(DbContextOptions<EventManagementDbContext> options) : base(options)
        {
        }

        public DbSet<RollDetail> RollDetails { get; set; }
        public DbSet<LoginDetail> LoginDetails { get; set; }
        public DbSet<UserRegistrationDetail> UserRegistrationDetails { get; set; }
        public DbSet<StateMaster> StateMasters { get; set; }
        public DbSet<CityMaster> CityMasters { get; set; }
        public DbSet<AreaMaster> AreaMasters { get; set; }
        public DbSet<EventTypeMaster> EventTypeMasters { get; set; }
        public DbSet<ServiceProvider> ServiceProviders { get; set; }
        public DbSet<ServiceCatalogItem> ServiceCatalogItems { get; set; }
        public DbSet<BookingCartDetail> BookingCartDetails { get; set; }
        public DbSet<BookingServiceDetail> BookingServiceDetails { get; set; }
        public DbSet<EventStatusMaster> EventStatusMasters { get; set; }
        public DbSet<PaymentDetail> PaymentDetails { get; set; }
        public DbSet<InquiryDetail> InquiryDetails { get; set; }
        public DbSet<FeedBackDetail> FeedBackDetails { get; set; }
        public DbSet<EventTimeMaster> EventTimeMasters { get; set; }
        public DbSet<ServiceProviderTypeMaster> ServiceProviderTypeMasters { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ServiceCatalogItem>()
                .HasOne(s => s.ServiceProvider)
                .WithMany(p => p.Services)
                .HasForeignKey(s => s.Service_Provider_Id_fk)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<BookingServiceDetail>()
                .HasOne(d => d.Booking)
                .WithMany(b => b.Services)
                .HasForeignKey(d => d.Booking_Id_fk)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<BookingServiceDetail>()
                .HasOne(d => d.ServiceCatalogItem)
                .WithMany(s => s.BookingServices)
                .HasForeignKey(d => d.Service_Catalog_Item_Id_fk)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<EventStatusMaster>()
                .HasOne(e => e.Booking)
                .WithMany(b => b.StatusUpdates)
                .HasForeignKey(e => e.Event_Booking_Cart_fk)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<EventStatusMaster>()
                .HasOne(e => e.ServicePersonLogin)
                .WithMany()
                .HasForeignKey(e => e.Service_Person_Id_fk)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<PaymentDetail>()
                .HasOne(p => p.Booking)
                .WithMany(b => b.Payments)
                .HasForeignKey(p => p.Booking_Id_fk)
                .OnDelete(DeleteBehavior.Cascade);

            base.OnModelCreating(modelBuilder);
        }
    }
}


