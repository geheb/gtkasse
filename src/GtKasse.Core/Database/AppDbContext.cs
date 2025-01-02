﻿using GtKasse.Core.Entities;
using Microsoft.AspNetCore.DataProtection.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;

namespace GtKasse.Core.Database;

public sealed class AppDbContext :
    IdentityDbContext<IdentityUserGuid, IdentityRoleGuid, Guid, IdentityUserClaimGuid, IdentityUserRoleGuid, IdentityUserLoginGuid, IdentityRoleClaimGuid, IdentityUserTokenGuid>,
    IDataProtectionKeyContext
{
    const string KeyType = "binary(16)";

    public DbSet<DataProtectionKey> DataProtectionKeys { get; set; } = null!;

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.HasCharSet(CharSet.Utf8Mb4, DelegationModes.ApplyToDatabases);

        modelBuilder.Entity<IdentityUserGuid>(eb =>
        {
            eb.Property(e => e.Id).HasColumnType(KeyType);
            eb.Property(e => e.Name).HasMaxLength(256);
            eb.Property(e => e.DebtorNumber).HasMaxLength(256);
            eb.Property(e => e.AddressNumber).HasMaxLength(256);

            eb.HasMany(e => e.UserRoles)
                .WithOne(e => e.User)
                .HasForeignKey(e => e.UserId)
                .IsRequired();

            eb.ToTable("users");
        });

        modelBuilder.Entity<IdentityRoleGuid>(eb =>
        {
            eb.Property(e => e.Id).HasColumnType(KeyType);

            eb.HasMany(e => e.UserRoles)
                .WithOne(e => e.Role)
                .HasForeignKey(e => e.RoleId)
                .IsRequired();

            eb.ToTable("roles");
        });

        modelBuilder.Entity<IdentityUserRoleGuid>(eb =>
        {
            eb.Property(e => e.UserId).HasColumnType(KeyType);
            eb.Property(e => e.RoleId).HasColumnType(KeyType);
            eb.ToTable("user_roles");
        });

        modelBuilder.Entity<IdentityUserLoginGuid>(eb =>
        {
            eb.Property(e => e.UserId).HasColumnType(KeyType);
            eb.ToTable("user_logins");
        });

        modelBuilder.Entity<IdentityUserTokenGuid>(eb =>
        {
            eb.Property(e => e.UserId).HasColumnType(KeyType);
            eb.ToTable("user_tokens");
        });

        modelBuilder.Entity<IdentityUserClaimGuid>(eb =>
        {
            eb.Property(e => e.Id).UseMySqlIdentityColumn();
            eb.Property(e => e.UserId).HasColumnType(KeyType);
            eb.ToTable("user_claims");
        });

        modelBuilder.Entity<IdentityRoleClaimGuid>(eb =>
        {
            eb.Property(e => e.Id).UseMySqlIdentityColumn();
            eb.Property(e => e.RoleId).HasColumnType(KeyType);
            eb.ToTable("role_claims");
        });

        modelBuilder.Entity<DataProtectionKey>(eb =>
        {
            eb.Property(e => e.Id).UseMySqlIdentityColumn();
            eb.ToTable("data_protection_keys");
        });

        modelBuilder.ApplyConfiguration(new RoleSeeder());

        modelBuilder.Entity<AccountNotification>(eb =>
        {
            eb.ToTable("account_notifications");
            eb.Property(e => e.Id).HasColumnType(KeyType).ValueGeneratedNever();
            eb.Property(e => e.UserId).HasColumnType(KeyType);
            eb.Property(e => e.Type).IsRequired();
            eb.Property(e => e.ReferenceId).HasColumnType(KeyType);

            eb.HasOne(e => e.User)
                .WithMany(e => e.AccountNotifications)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.NoAction)
                .IsRequired(false);

            eb.HasIndex(e => new { e.UserId, e.Type, e.CreatedOn });

            eb.HasIndex(e => new { e.ReferenceId, e.Type });
        });

        modelBuilder.Entity<Food>(eb =>
        {
            eb.ToTable("foods");
            eb.Property(e => e.Id).HasColumnType(KeyType).ValueGeneratedNever();
            eb.Property(e => e.Name).IsRequired().HasMaxLength(128);
            eb.Property(e => e.Price).HasColumnType("decimal(6,2)").IsRequired();
            eb.Property(e => e.Type).IsRequired();
            eb.Property(e => e.FoodListId).HasColumnType(KeyType);

            eb.HasOne(e => e.FoodList)
                .WithMany(e => e.Foods)
                .HasForeignKey(e => e.FoodListId)
                .OnDelete(DeleteBehavior.NoAction)
                .IsRequired(false);
        });

        modelBuilder.Entity<FoodList>(eb =>
        {
            eb.ToTable("food_lists");
            eb.Property(e => e.Id).HasColumnType(KeyType).ValueGeneratedNever();
            eb.Property(e => e.Name).IsRequired().HasMaxLength(128);
            eb.Property(e => e.ValidFrom).IsRequired();

            eb.HasIndex(e => e.ValidFrom);
        });

        modelBuilder.Entity<Booking>(eb =>
        {
            eb.ToTable("bookings");
            eb.Property(e => e.Id).HasColumnType(KeyType).ValueGeneratedNever();
            eb.Property(e => e.UserId).HasColumnType(KeyType);
            eb.Property(e => e.FoodId).HasColumnType(KeyType);
            eb.Property(e => e.Status).IsRequired();
            eb.Property(e => e.Count).IsRequired();
            eb.Property(e => e.BookedOn).IsRequired();
            eb.Property(e => e.InvoiceId).HasColumnType(KeyType);

            eb.HasOne(e => e.User)
                .WithMany(e => e.Bookings)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.NoAction)
                .IsRequired(false);

            eb.HasOne(e => e.Food)
                .WithMany(e => e.Bookings)
                .HasForeignKey(e => e.FoodId)
                .OnDelete(DeleteBehavior.NoAction)
                .IsRequired(false);

            eb.HasOne(e => e.Invoice)
                .WithMany(e => e.Bookings)
                .HasForeignKey(e => e.InvoiceId)
                .OnDelete(DeleteBehavior.NoAction)
                .IsRequired(false);

            eb.HasIndex(e => new { e.UserId, e.BookedOn});
        });

        modelBuilder.Entity<Invoice>(eb =>
        {
            eb.ToTable("invoices");
            eb.Property(e => e.Id).HasColumnType(KeyType).ValueGeneratedNever();
            eb.Property(e => e.UserId).HasColumnType(KeyType);
            eb.Property(e => e.CreatedOn).IsRequired();
            eb.Property(e => e.Total).HasColumnType("decimal(6,2)").IsRequired();
            eb.Property(e => e.Status).IsRequired();

            eb.HasOne(e => e.InvoicePeriod)
                .WithMany(e => e.Invoices)
                .HasForeignKey(e => e.InvoicePeriodId)
                .OnDelete(DeleteBehavior.NoAction)
                .IsRequired(false);

            eb.HasOne(e => e.User)
                .WithMany(e => e.Invoices)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.NoAction)
                .IsRequired(false);
        });

        modelBuilder.Entity<InvoicePeriod>(eb =>
        {
            eb.ToTable("invoice_periods");
            eb.Property(e => e.Id).HasColumnType(KeyType).ValueGeneratedNever();
            eb.Property(e => e.Description).IsRequired();
            eb.Property(e => e.From).IsRequired();
            eb.Property(e => e.To).IsRequired();

            eb.HasIndex(e => e.To);
        });

        modelBuilder.Entity<Trip>(eb =>
        {
            eb.ToTable("trips");
            eb.Property(e => e.Id).HasColumnType(KeyType).ValueGeneratedNever();
            eb.Property(e => e.UserId).HasColumnType(KeyType);
            eb.Property(e => e.Start).IsRequired();
            eb.Property(e => e.End).IsRequired();
            eb.Property(e => e.Target).IsRequired();
            eb.Property(e => e.MaxBookings).IsRequired();
            eb.Property(e => e.BookingStart).IsRequired();
            eb.Property(e => e.BookingEnd).IsRequired();

            eb.HasOne(e => e.User)
                .WithMany(e => e.Trips)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.NoAction)
                .IsRequired(false);

            eb.HasIndex(e => new { e.Start, e.End });
        });

        modelBuilder.Entity<TripBooking>(eb =>
        {
            eb.ToTable("trip_bookings");
            eb.Property(e => e.Id).HasColumnType(KeyType).ValueGeneratedNever();
            eb.Property(e => e.UserId).HasColumnType(KeyType);
            eb.Property(e => e.TripId).HasColumnType(KeyType);
            eb.Property(e => e.BookedOn).IsRequired();
            eb.Property(e => e.Name).HasMaxLength(256);

            eb.HasOne(e => e.User)
                .WithMany(e => e.TripBookings)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.NoAction)
                .IsRequired(false);

            eb.HasOne(e => e.Trip)
                .WithMany(e => e.TripBookings)
                .HasForeignKey(e => e.TripId)
                .OnDelete(DeleteBehavior.NoAction)
                .IsRequired(false);
        });

        modelBuilder.Entity<WikiArticle>(eb =>
        {
            eb.ToTable("wiki_articles");
            eb.Property(e => e.Id).HasColumnType(KeyType).ValueGeneratedNever();
            eb.Property(e => e.UserId).HasColumnType(KeyType);
            eb.Property(e => e.CreatedOn).IsRequired();
            eb.Property(e => e.Identifier).IsRequired().HasMaxLength(8);
            eb.Property(e => e.Title).IsRequired().HasMaxLength(256);

            eb.HasOne(e => e.User)
                .WithMany(e => e.WikiArticles)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.NoAction)
                .IsRequired(false);

            eb.HasIndex(e => e.Identifier).IsUnique();
        });

        modelBuilder.Entity<TripChat>(eb =>
        {
            eb.ToTable("trip_chats");
            eb.Property(e => e.Id).HasColumnType(KeyType).ValueGeneratedNever();
            eb.Property(e => e.UserId).HasColumnType(KeyType);
            eb.Property(e => e.TripId).HasColumnType(KeyType);
            eb.Property(e => e.CreatedOn).IsRequired();
            eb.Property(e => e.Message).IsRequired().HasMaxLength(256);

            eb.HasOne(e => e.User)
                .WithMany(e => e.TripChats)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.NoAction)
                .IsRequired(false);
        });

        modelBuilder.Entity<Vehicle>(eb =>
        {
            eb.ToTable("vehicles");
            eb.Property(e => e.Id).HasColumnType(KeyType).ValueGeneratedNever();
            eb.Property(e => e.Name).IsRequired().HasMaxLength(64);
            eb.Property(e => e.Identifier).IsRequired().HasMaxLength(12);

            eb.HasMany(e => e.Bookings)
                .WithOne(e => e.Vehicle)
                .HasForeignKey(e => e.VehicleId)
                .OnDelete(DeleteBehavior.NoAction)
                .IsRequired(false);
        });

        modelBuilder.Entity<VehicleBooking>(eb =>
        {
            eb.ToTable("vehicle_bookings");
            eb.Property(e => e.Id).HasColumnType(KeyType).ValueGeneratedNever();
            eb.Property(e => e.UserId).HasColumnType(KeyType);
            eb.Property(e => e.VehicleId).HasColumnType(KeyType);
            eb.Property(e => e.Start).IsRequired();
            eb.Property(e => e.End).IsRequired();
            eb.Property(e => e.BookedOn).IsRequired();
            eb.Property(e => e.Purpose).IsRequired().HasMaxLength(128);

            eb.HasOne(e => e.User)
                .WithMany(e => e.VehicleBookings)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.NoAction)
                .IsRequired(false);

            eb.HasOne(e => e.Vehicle)
                .WithMany(e => e.Bookings)
                .HasForeignKey(e => e.VehicleId)
                .OnDelete(DeleteBehavior.NoAction)
                .IsRequired(false);
        });

        modelBuilder.Entity<Tryout>(eb =>
        {
            eb.ToTable("tryouts");
            eb.Property(e => e.Id).HasColumnType(KeyType).ValueGeneratedNever();
            eb.Property(e => e.UserId).HasColumnType(KeyType);
            eb.Property(e => e.Date).IsRequired();
            eb.Property(e => e.MaxBookings).IsRequired();
            eb.Property(e => e.BookingStart).IsRequired();
            eb.Property(e => e.BookingEnd).IsRequired();
            
            eb.HasOne(e => e.User)
                .WithMany(e => e.Tryouts)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.NoAction)
                .IsRequired(false);
        });

        modelBuilder.Entity<TryoutBooking>(eb =>
        {
            eb.ToTable("tryout_bookings");
            eb.Property(e => e.Id).HasColumnType(KeyType).ValueGeneratedNever();
            eb.Property(e => e.UserId).HasColumnType(KeyType);
            eb.Property(e => e.TryoutId).HasColumnType(KeyType);
            eb.Property(e => e.BookedOn).IsRequired();
            eb.Property(e => e.Name).HasMaxLength(256);

            eb.HasOne(e => e.User)
                .WithMany(e => e.TryoutBookings)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.NoAction)
                .IsRequired(false);

            eb.HasOne(e => e.Tryout)
                .WithMany(e => e.TryoutBookings)
                .HasForeignKey(e => e.TryoutId)
                .OnDelete(DeleteBehavior.NoAction)
                .IsRequired(false);
        });

        modelBuilder.Entity<Boat>(eb =>
        {
            eb.ToTable("boats");
            eb.Property(e => e.Id).HasColumnType(KeyType).ValueGeneratedNever();
            eb.Property(e => e.Name).IsRequired().HasMaxLength(64);
            eb.Property(e => e.Identifier).IsRequired().HasMaxLength(8);
            eb.Property(e => e.Location).IsRequired().HasMaxLength(64);

            eb.HasMany(e => e.BoatRentals)
                .WithOne(e => e.Boat)
                .HasForeignKey(e => e.BoatId)
                .OnDelete(DeleteBehavior.NoAction)
                .IsRequired(false);
        });

        modelBuilder.Entity<BoatRental>(eb =>
        {
            eb.ToTable("boat_rentals");
            eb.Property(e => e.Id).HasColumnType(KeyType).ValueGeneratedNever();
            eb.Property(e => e.UserId).HasColumnType(KeyType);
            eb.Property(e => e.BoatId).HasColumnType(KeyType);
            eb.Property(e => e.Start).IsRequired();
            eb.Property(e => e.End).IsRequired();
            eb.Property(e => e.Purpose).IsRequired().HasMaxLength(128);

            eb.HasOne(e => e.User)
                .WithMany(e => e.BoatRentals)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.NoAction)
                .IsRequired(false);

            eb.HasOne(e => e.Boat)
                .WithMany(e => e.BoatRentals)
                .HasForeignKey(e => e.BoatId)
                .OnDelete(DeleteBehavior.NoAction)
                .IsRequired(false);
        });
    }
}
