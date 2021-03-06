using DentistryManagement.Server.Data.DatabaseSeeders;
using DentistryManagement.Server.Models;
using IdentityServer4.EntityFramework.Options;
using Microsoft.AspNetCore.ApiAuthorization.IdentityServer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace DentistryManagement.Server.Data
{
    public class ApplicationDbContext : ApiAuthorizationDbContext<ApplicationUser>
    {
        public ApplicationDbContext(
            DbContextOptions options,
            IOptions<OperationalStoreOptions> operationalStoreOptions) : base(options, operationalStoreOptions)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            #region ApplicationUser

            builder.Entity<ApplicationUser>(b => {
                b.HasMany(x => x.UserRoles).WithOne().HasForeignKey(ur => ur.UserId).IsRequired();
                b.HasMany(t => t.TreatmentHistories).WithOne(th => th.User).HasForeignKey(th => th.UserId).IsRequired();
                b.HasMany(t => t.Schedule).WithOne(s => s.User).HasForeignKey(s => s.UserId).IsRequired();
                b.HasMany(t => t.Comments).WithOne(c => c.User).HasForeignKey(th => th.UserId);
            });

            builder.Entity<ApplicationRole>(b =>
            {
                b.HasMany(ur => ur.UserRoles).WithOne().HasForeignKey(ur => ur.RoleId).IsRequired();
            });

            builder.Entity<ApplicationUserRole>(b =>
            {
                b.HasOne(ur => ur.Role).WithMany(r => r.UserRoles).HasForeignKey(ur => ur.RoleId);
                b.HasOne(ur => ur.User).WithMany(r => r.UserRoles).HasForeignKey(ur => ur.UserId);
            });

            #endregion

            #region Affiliate

            builder.Entity<Affiliate>(b => {
                b.HasMany(u => u.Users).WithOne(a => a.Affiliate);
                b.HasOne(a => a.Address).WithOne(a => a.Affiliate).HasForeignKey<Address>(a => a.AffiliateId);
                b.HasMany(t => t.TreatmentHistories).WithOne(a => a.Affiliate);
            });

            #endregion

            #region Patient

            builder.Entity<Patient>(b =>
            {
                b.HasOne(p => p.MedicalChart).WithOne(mc => mc.Patient).HasForeignKey<MedicalChart>(mc => mc.PatientId);
                b.HasMany(p => p.Schedule).WithOne(c => c.Patient).HasForeignKey(th => th.PatientId);
            });

            #endregion

            #region MedicalChart

            builder.Entity<MedicalChart>(b =>
            {
                b.HasMany(t => t.Teeth).WithOne(mc => mc.MedicalChart);
                b.HasMany(t => t.Allergies).WithOne(mc => mc.MedicalChart);
                b.HasMany(t => t.Files).WithOne(mc => mc.MedicalChart);
                b.HasMany(t => t.TreatmentHistories).WithOne(mc => mc.MedicalChart);
            });

            #endregion

            #region ToothDisease

            builder.Entity<ToothDisease>().HasKey(td => new { td.DiseaseId, td.ToothId });

            builder.Entity<ToothDisease>().HasOne(td => td.Disease).WithMany(d => d.ToothDiseases).HasForeignKey(td => td.DiseaseId);

            builder.Entity<ToothDisease>().HasOne(td => td.Tooth).WithMany(d => d.ToothDiseases).HasForeignKey(td => td.ToothId);

            #endregion

            #region Treatment

            builder.Entity<Treatment>(b => {
                b.HasMany(t => t.TreatmentHistories).WithOne(th => th.Treatment);
            });

            #endregion

            #region Tooth

            builder.Entity<Tooth>(b => {
                b.HasMany(t => t.Comments).WithOne(c => c.Tooth);
                b.HasMany(t => t.TreatmentHistories).WithOne(c => c.Tooth);
            });

            #endregion

            #region Seeds

            RoleSeeder.Seed(builder);
            AffiliateSeeder.Seed(builder);
            UserSeeder.Seed(builder);
            UserRoleSeed.Seed(builder);

            #endregion
        }

        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<ApplicationRole> ApplicationRole { get; set; }
        public DbSet<ApplicationUserRole> ApplicationUserRole { get; set; }
        public DbSet<Affiliate> Affiliates { get; set; }
        public DbSet<Address> Addresses { get; set; }
        public DbSet<Patient> Patients { get; set; }
        public DbSet<MedicalChart> MedicalCharts { get; set; }
        public DbSet<Tooth> Teeth { get; set; }
        public DbSet<Disease> Diseases { get; set; }
        public DbSet<ToothDisease> ToothDiseases { get; set; }
        public DbSet<Allergy> Allergies { get; set; }
        public DbSet<File> Files { get; set; }
        public DbSet<Treatment> Treatments { get; set; }
        public DbSet<TreatmentHistory> TreatmentHistories { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Schedule> Schedule { get; set; }
    }
}
