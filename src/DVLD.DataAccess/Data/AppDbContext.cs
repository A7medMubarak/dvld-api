using DVLD.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace DVLD.DataAccess.Data
{
    public class AppDbContext : DbContext
    {
       
        public AppDbContext(DbContextOptions options)
            : base(options) 
        {
            
        }

        //Represent the collection of all entities
        public DbSet<Application> Applications { get; set; } = null!;
        public DbSet<ApplicationType> ApplicationTypes {  get; set; } = null!;
        public DbSet<Country> Countries {  get; set; } = null!;
        public DbSet<DetainedLicense> DetainedLicenses {  get; set; } = null!;
        public DbSet<Driver> Drivers {  get; set; } = null!;
        public DbSet<InternationalLicense> InternationalLicenses {  get; set; } = null!;
        public DbSet<LicenseClass> LicenseClasses {  get; set; } = null!;
        public DbSet<License> Licenses {  get; set; } = null!;
        public DbSet<LocalDrivingLicenseApplication> LocalDrivingLicenseApplications {  get; set; } = null!;
        public DbSet<Person> People {  get; set; } = null!;
        public DbSet<TestAppointment> TestAppointments {  get; set; } = null!;
        public DbSet<Test> Tests {  get; set; } = null!;
        public DbSet<TestType> TestTypes {  get; set; } = null!;
        public DbSet<User> Users {  get; set; } = null!;
        public DbSet<RefreshToken> RefreshTokens { get; set; } = null!;


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        }

    }
}
