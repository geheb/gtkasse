using GtKasse.Core.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace GtKasse.Core.Database;

public sealed class AppDbContext :
    IdentityDbContext<IdentityUserGuid, IdentityRoleGuid, Guid, IdentityUserClaimGuid, IdentityUserRoleGuid, IdentityUserLoginGuid, IdentityRoleClaimGuid, IdentityUserTokenGuid>
{
    private sealed class ShortGuidConverter : ValueConverter<Guid, string>
    {
        public ShortGuidConverter() : 
            base(v => v.ToString("N"), v => new Guid(v))
        {
        }
    }

    private sealed class DateTimeOffsetToUtcConverter : ValueConverter<DateTimeOffset, DateTime>
    {
        public DateTimeOffsetToUtcConverter() :
            base(v => v.UtcDateTime, v => new DateTimeOffset(v, TimeSpan.Zero))
        {
        }
    }

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder
            .Properties<Guid>()
            .HaveConversion<ShortGuidConverter>()
            .HaveMaxLength(32);
        
        configurationBuilder
            .Properties<DateTimeOffset>()
            .HaveConversion<DateTimeOffsetToUtcConverter>();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<IdentityUserGuid>(eb =>
        {
            eb.Property(e => e.Id).ValueGeneratedNever();
            eb.Property(e => e.Name).HasMaxLength(256);
            eb.Property(e => e.DebtorNumber).HasMaxLength(256);
            eb.Property(e => e.AddressNumber).HasMaxLength(256);
            eb.ToTable("identity_users");

            eb.HasMany(e => e.UserRoles)
                .WithOne(e => e.User)
                .HasForeignKey(e => e.UserId)
                .IsRequired();

            var index = eb.HasIndex(e => e.NormalizedEmail).Metadata;
            eb.Metadata.RemoveIndex(index);
            index = eb.HasIndex(e => e.NormalizedUserName).Metadata;
            eb.Metadata.RemoveIndex(index);

            eb.HasIndex(e => e.NormalizedEmail, "IX_identity_users_NormalizedEmail").IsUnique();
            eb.HasIndex(e => e.NormalizedUserName, "IX_identity_users_NormalizedUserName").IsUnique();
        });

        modelBuilder.Entity<IdentityRoleGuid>(eb =>
        {
            eb.Property(e => e.Id).ValueGeneratedNever();
            eb.ToTable("identity_roles");

            eb.HasMany(e => e.UserRoles)
                .WithOne(e => e.Role)
                .HasForeignKey(e => e.RoleId)
                .IsRequired();

            var index = eb.HasIndex(e => e.NormalizedName).Metadata;
            eb.Metadata.RemoveIndex(index);

            eb.HasIndex(e => e.NormalizedName, "IX_identity_roles_NormalizedName").IsUnique();
        });

        modelBuilder.Entity<IdentityUserRoleGuid>(eb =>
        {
            eb.Property(e => e.UserId).ValueGeneratedNever();
            eb.Property(e => e.RoleId).ValueGeneratedNever();
            eb.ToTable("identity_user_roles");
        });

        modelBuilder.Entity<IdentityUserLoginGuid>(eb =>
        {
            eb.Property(e => e.UserId).ValueGeneratedNever();
            eb.ToTable("identity_user_logins");
        });

        modelBuilder.Entity<IdentityUserTokenGuid>(eb =>
        {
            eb.Property(e => e.UserId).ValueGeneratedNever();
            eb.ToTable("identity_user_tokens");
        });

        modelBuilder.Entity<IdentityUserClaimGuid>(eb =>
        {
            eb.Property(e => e.Id).ValueGeneratedOnAdd();
            eb.Property(e => e.UserId).ValueGeneratedNever();

            eb.ToTable("identity_user_claims");
        });

        modelBuilder.Entity<IdentityRoleClaimGuid>(eb =>
        {
            eb.Property(e => e.Id).ValueGeneratedNever();
            eb.Property(e => e.RoleId).ValueGeneratedNever();
            eb.ToTable("identity_role_claims");
        });

        modelBuilder.ApplyConfiguration(new RoleSeeder());

        modelBuilder.Entity<EmailQueue>(eb =>
        {
            eb.HasIndex(e => new { e.Sent, e.Created, e.IsPrio });
        });

        modelBuilder.Entity<FoodList>();
        modelBuilder.Entity<Food>();
        modelBuilder.Entity<FoodBooking>(eb =>
        {
            eb.HasIndex(e => new { e.UserId, e.BookedOn });

        });

        modelBuilder.Entity<InvoicePeriod>(eb =>
        {
            eb.HasIndex(e => e.To);
        });
        modelBuilder.Entity<Invoice>();

        modelBuilder.Entity<WikiArticle>(eb =>
        {
            eb.HasIndex(e => e.Identifier).IsUnique();
        });

        modelBuilder.Entity<Boat>();
        modelBuilder.Entity<BoatRental>();

        modelBuilder.Entity<Vehicle>();
        modelBuilder.Entity<VehicleBooking>();

        modelBuilder.Entity<ClubhouseBooking>();

        modelBuilder.Entity<Trip>(eb =>
        {
            eb.HasIndex(e => new { e.Start, e.End });
        });
        modelBuilder.Entity<TripBooking>();
        modelBuilder.Entity<TripChat>();

        modelBuilder.Entity<Tryout>();
        modelBuilder.Entity<TryoutBooking>();
        modelBuilder.Entity<TryoutChat>();

        modelBuilder.Entity<Mailing>();
        modelBuilder.Entity<MyMailing>();
    }
}
