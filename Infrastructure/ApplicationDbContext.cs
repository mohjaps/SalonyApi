
using Core.TableDb;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;


namespace Infrastructure
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationDbUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public ApplicationDbContext()
        {

        }
        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    optionsBuilder.UseSqlServer("Data Source=135.181.0.148;Initial Catalog=RashidCleanDb;User ID=RashidCleanUser;Password=4I3zcf0_; MultipleActiveResultSets = true;");
        //}

        public DbSet<Carts> Carts { get; set; }
        public DbSet<Categories> Categories { get; set; }
        public DbSet<Cities> Cities { get; set; }
        public DbSet<ContactUs> ContactUs { get; set; }
        public DbSet<Districts> Districts { get; set; }
        public DbSet<MainServices> MainServices { get; set; }
        public DbSet<Offers> Offers { get; set; }
        public DbSet<Orders> Orders { get; set; }
        public DbSet<OrderServices> OrderServices { get; set; }
        public DbSet<ProviderAditionalData> ProviderAditionalData { get; set; }
        public DbSet<SalonImages> SalonImages { get; set; }
        public DbSet<Settings> Settings { get; set; }
        public DbSet<Sliders> Sliders { get; set; }
        public DbSet<SubServices> SubServices { get; set; }
        public DbSet<DeviceIds> DeviceIds { get; set; }
        public DbSet<Copons> Copons { get; set; }
        public DbSet<Notifications> Notifications { get; set; }
        public DbSet<Branches> Branches { get; set; }
        public DbSet<Messages> Messages { get; set; }
        public DbSet<HistoryNotify> HistoryNotify { get; set; }
        public DbSet<BankAccounts> BankAccounts { get; set; }
        public DbSet<Employees> Employees { get; set; }
        public DbSet<ServiceDelivery> ServicesDelivery { get; set; }
        public DbSet<Worker> Workers { get; set; }
        public DbSet<WorkerEvaluation> WorkerEvaluations { get; set; }
        public DbSet<SallonEvaluation> SallonEvaluations { get; set; }
        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<Vacation> Vacations { get; set; }


        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.HasDefaultSchema("ReqqaDBUser");

            builder.Entity<ApplicationDbUser>().HasQueryFilter(a => !a.isDeleted);
            builder.Entity<Notifications>().HasQueryFilter(a => !a.FK_User.isDeleted);
            builder.Entity<ProviderAditionalData>().HasQueryFilter(a => !a.FK_User.isDeleted);
            builder.Entity<Orders>().HasQueryFilter(a => !a.FK_User.isDeleted);

        }


    }
}
