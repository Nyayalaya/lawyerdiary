using CourtApp.Infrastructure.Identity.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Text.Json;

namespace CourtApp.Infrastructure.DbContexts
{
    public class IdentityContext : IdentityDbContext<ApplicationUser>
    {

        public IdentityContext(DbContextOptions<IdentityContext> options) : base(options)
        {
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
        }
        //public DbSet<Demographic> Lawyers { get; set; }
        public DbSet<OperatorUser> LawyerUsers { get; set; }
        public DbSet<CorporateUser> Corporates { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.HasDefaultSchema("Identity");
            builder.Entity<ApplicationUser>(entity =>
            {
                entity.ToTable(name: "Users");
            });

            // configure Application User and Demographic relationship.
            //builder.Entity<ApplicationUser>().
            //    HasOne(e => e.Demographic).WithOne(d => d.User)
            //    .HasForeignKey<ApplicationUser>(a => a.DemographicId);

            //Configure Application User and operator Relationship.
            builder.Entity<OperatorUser>()
                .HasOne(o => o.Lawyer)
                .WithMany(l => l.Operators);

            builder.Entity<IdentityRole>(entity =>
            {
                entity.ToTable(name: "Roles");
            });
            builder.Entity<IdentityUserRole<string>>(entity =>
            {
                entity.ToTable("UserRoles");
            });

            builder.Entity<IdentityUserClaim<string>>(entity =>
            {
                entity.ToTable("UserClaims");
            });

            builder.Entity<IdentityUserLogin<string>>(entity =>
            {
                entity.ToTable("UserLogins");
            });

            builder.Entity<IdentityRoleClaim<string>>(entity =>
            {
                entity.ToTable("RoleClaims");
            });

            builder.Entity<IdentityUserToken<string>>(entity =>
            {
                entity.ToTable("UserTokens");
            });

            builder.Entity<ApplicationUser>(entity =>
            {
                entity.Property(e => e.ProfessionalInfo)
                 .HasConversion(
                     v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null),
                     v => JsonSerializer.Deserialize<ProfessionalInfo>(v, (JsonSerializerOptions)null))
                 .HasColumnType("jsonb");

                entity.Property(e => e.AddressInfo)
                  .HasConversion(
                      v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null),
                      v => JsonSerializer.Deserialize<AddressInfo>(v, (JsonSerializerOptions)null))
                  .HasColumnType("jsonb");

                entity.Property(e => e.ContactInfo)
                  .HasConversion(
                      v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null),
                      v => JsonSerializer.Deserialize<ContactInfo>(v, (JsonSerializerOptions)null))
                  .HasColumnType("jsonb");

                entity.Property(e => e.WorkLocInfo)
                  .HasConversion(
                      v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null),
                      v => JsonSerializer.Deserialize<WorkLocation>(v, (JsonSerializerOptions)null))
                  .HasColumnType("jsonb");
            });

            builder.Entity<Demographic>(entity =>
            {
                entity.Property(e => e.ProfessionalInfo)
                  .HasConversion(
                      v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null),
                      v => JsonSerializer.Deserialize<ProfessionalInfo>(v, (JsonSerializerOptions)null))
                  .HasColumnType("jsonb");

                entity.Property(e => e.AddressInfo)
                  .HasConversion(
                      v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null),
                      v => JsonSerializer.Deserialize<AddressInfo>(v, (JsonSerializerOptions)null))
                  .HasColumnType("jsonb");

                entity.Property(e => e.ContactInfo)
                  .HasConversion(
                      v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null),
                      v => JsonSerializer.Deserialize<ContactInfo>(v, (JsonSerializerOptions)null))
                  .HasColumnType("jsonb");

                entity.Property(e => e.WorkLocInfo)
                  .HasConversion(
                      v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null),
                      v => JsonSerializer.Deserialize<WorkLocation>(v, (JsonSerializerOptions)null))
                  .HasColumnType("jsonb");
            });

            builder.Entity<OperatorUser>(entity =>
            {
                entity.Property(e => e.AddressInfo)
                 .HasConversion(
                     v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null),
                     v => JsonSerializer.Deserialize<AddressInfo>(v, (JsonSerializerOptions)null))
                 .HasColumnType("jsonb");
            });

            builder.Entity<CorporateUser>()
               .HasOne(c => c.User)
               .WithOne()
               .HasForeignKey<CorporateUser>(c => c.Id)
               .OnDelete(DeleteBehavior.Cascade);
        }
    }
}