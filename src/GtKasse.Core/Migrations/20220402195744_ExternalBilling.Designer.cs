﻿// <auto-generated />
using System;
using GtKasse.Core.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace GtKasse.Core.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20220402195744_ExternalBilling")]
    partial class ExternalBilling
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.3")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            MySqlModelBuilderExtensions.HasCharSet(modelBuilder, "utf8mb4", DelegationModes.ApplyToDatabases);

            modelBuilder.Entity("GtKasse.Core.Entities.AccountNotification", b =>
                {
                    b.Property<byte[]>("Id")
                        .HasColumnType("binary(16)");

                    b.Property<string>("CallbackUrl")
                        .HasColumnType("longtext");

                    b.Property<DateTimeOffset>("CreatedOn")
                        .HasColumnType("datetime(6)");

                    b.Property<byte[]>("ReferenceId")
                        .HasColumnType("binary(16)");

                    b.Property<DateTimeOffset?>("SentOn")
                        .HasColumnType("datetime(6)");

                    b.Property<int>("Type")
                        .HasColumnType("int");

                    b.Property<byte[]>("UserId")
                        .HasColumnType("binary(16)");

                    b.HasKey("Id");

                    b.HasIndex("ReferenceId", "Type");

                    b.HasIndex("UserId", "Type", "CreatedOn");

                    b.ToTable("account_notifications", (string)null);
                });

            modelBuilder.Entity("GtKasse.Core.Entities.IdentityRoleGuid", b =>
                {
                    b.Property<byte[]>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("binary(16)");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("longtext");

                    b.Property<string>("Name")
                        .HasMaxLength(256)
                        .HasColumnType("varchar(256)");

                    b.Property<string>("NormalizedName")
                        .HasMaxLength(256)
                        .HasColumnType("varchar(256)");

                    b.HasKey("Id");

                    b.HasIndex("NormalizedName")
                        .IsUnique()
                        .HasDatabaseName("RoleNameIndex");

                    b.ToTable("roles", (string)null);

                    b.HasData(
                        new
                        {
                            Id = new byte[] { 191, 247, 188, 251, 191, 140, 15, 68, 147, 156, 246, 118, 49, 16, 154, 160 },
                            ConcurrencyStamp = "CFAD6F62-EEAA-4ECD-B847-0762C704EC45",
                            Name = "administrator",
                            NormalizedName = "ADMINISTRATOR"
                        },
                        new
                        {
                            Id = new byte[] { 118, 69, 182, 31, 144, 63, 233, 76, 140, 58, 207, 161, 63, 88, 113, 103 },
                            ConcurrencyStamp = "6EE38FDC-5CE5-42FD-A5A5-168573DB2F86",
                            Name = "treasurer",
                            NormalizedName = "TREASURER"
                        },
                        new
                        {
                            Id = new byte[] { 224, 7, 77, 29, 174, 64, 203, 69, 164, 246, 57, 118, 114, 243, 96, 91 },
                            ConcurrencyStamp = "995137CB-BD7B-4747-884E-A7467F3C0A3A",
                            Name = "kitchen",
                            NormalizedName = "KITCHEN"
                        },
                        new
                        {
                            Id = new byte[] { 165, 13, 169, 3, 240, 171, 233, 69, 155, 128, 205, 172, 121, 11, 113, 5 },
                            ConcurrencyStamp = "956824FE-2F13-4919-B2D2-0E60BECFCA12",
                            Name = "member",
                            NormalizedName = "MEMBER"
                        },
                        new
                        {
                            Id = new byte[] { 167, 240, 198, 157, 22, 113, 32, 72, 139, 118, 79, 32, 183, 48, 83, 238 },
                            ConcurrencyStamp = "3096E774-529C-41B4-8CD3-355E0D2C930D",
                            Name = "interested",
                            NormalizedName = "INTERESTED"
                        },
                        new
                        {
                            Id = new byte[] { 236, 155, 220, 45, 150, 235, 213, 76, 148, 180, 221, 13, 153, 161, 150, 100 },
                            ConcurrencyStamp = "F3BFC39C-70B7-4E91-A70B-131925290446",
                            Name = "tripmanager",
                            NormalizedName = "TRIPMANAGER"
                        });
                });

            modelBuilder.Entity("GtKasse.Core.Entities.IdentityRoleClaimGuid", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("ClaimType")
                        .HasColumnType("longtext");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("longtext");

                    b.Property<byte[]>("RoleId")
                        .IsRequired()
                        .HasColumnType("binary(16)");

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("role_claims", (string)null);
                });

            modelBuilder.Entity("GtKasse.Core.Entities.IdentityUserGuid", b =>
                {
                    b.Property<byte[]>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("binary(16)");

                    b.Property<int>("AccessFailedCount")
                        .HasColumnType("int");

                    b.Property<string>("AddressNumber")
                        .HasMaxLength(256)
                        .HasColumnType("varchar(256)");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("longtext");

                    b.Property<string>("DebtorNumber")
                        .HasMaxLength(256)
                        .HasColumnType("varchar(256)");

                    b.Property<string>("Email")
                        .HasMaxLength(256)
                        .HasColumnType("varchar(256)");

                    b.Property<bool>("EmailConfirmed")
                        .HasColumnType("tinyint(1)");

                    b.Property<DateTimeOffset?>("LastLogin")
                        .HasColumnType("datetime(6)");

                    b.Property<DateTimeOffset?>("LeftOn")
                        .HasColumnType("datetime(6)");

                    b.Property<bool>("LockoutEnabled")
                        .HasColumnType("tinyint(1)");

                    b.Property<DateTimeOffset?>("LockoutEnd")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("Name")
                        .HasMaxLength(256)
                        .HasColumnType("varchar(256)");

                    b.Property<string>("NormalizedEmail")
                        .HasMaxLength(256)
                        .HasColumnType("varchar(256)");

                    b.Property<string>("NormalizedUserName")
                        .HasMaxLength(256)
                        .HasColumnType("varchar(256)");

                    b.Property<string>("PasswordHash")
                        .HasColumnType("longtext");

                    b.Property<string>("PhoneNumber")
                        .HasColumnType("longtext");

                    b.Property<bool>("PhoneNumberConfirmed")
                        .HasColumnType("tinyint(1)");

                    b.Property<string>("SecurityStamp")
                        .HasColumnType("longtext");

                    b.Property<bool>("TwoFactorEnabled")
                        .HasColumnType("tinyint(1)");

                    b.Property<string>("UserName")
                        .HasMaxLength(256)
                        .HasColumnType("varchar(256)");

                    b.HasKey("Id");

                    b.HasIndex("NormalizedEmail")
                        .HasDatabaseName("EmailIndex");

                    b.HasIndex("NormalizedUserName")
                        .IsUnique()
                        .HasDatabaseName("UserNameIndex");

                    b.ToTable("users", (string)null);
                });

            modelBuilder.Entity("GtKasse.Core.Entities.IdentityUserClaimGuid", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("ClaimType")
                        .HasColumnType("longtext");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("longtext");

                    b.Property<byte[]>("UserId")
                        .IsRequired()
                        .HasColumnType("binary(16)");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("user_claims", (string)null);
                });

            modelBuilder.Entity("GtKasse.Core.Entities.IdentityUserLoginGuid", b =>
                {
                    b.Property<string>("LoginProvider")
                        .HasColumnType("varchar(255)");

                    b.Property<string>("ProviderKey")
                        .HasColumnType("varchar(255)");

                    b.Property<string>("ProviderDisplayName")
                        .HasColumnType("longtext");

                    b.Property<byte[]>("UserId")
                        .IsRequired()
                        .HasColumnType("binary(16)");

                    b.HasKey("LoginProvider", "ProviderKey");

                    b.HasIndex("UserId");

                    b.ToTable("user_logins", (string)null);
                });

            modelBuilder.Entity("GtKasse.Core.Entities.IdentityUserRoleGuid", b =>
                {
                    b.Property<byte[]>("UserId")
                        .HasColumnType("binary(16)");

                    b.Property<byte[]>("RoleId")
                        .HasColumnType("binary(16)");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.ToTable("user_roles", (string)null);
                });

            modelBuilder.Entity("GtKasse.Core.Entities.IdentityUserTokenGuid", b =>
                {
                    b.Property<byte[]>("UserId")
                        .HasColumnType("binary(16)");

                    b.Property<string>("LoginProvider")
                        .HasColumnType("varchar(255)");

                    b.Property<string>("Name")
                        .HasColumnType("varchar(255)");

                    b.Property<string>("Value")
                        .HasColumnType("longtext");

                    b.HasKey("UserId", "LoginProvider", "Name");

                    b.ToTable("user_tokens", (string)null);
                });

            modelBuilder.Entity("GtKasse.Core.Entities.Booking", b =>
                {
                    b.Property<byte[]>("Id")
                        .HasColumnType("binary(16)");

                    b.Property<DateTimeOffset>("BookedOn")
                        .HasColumnType("datetime(6)");

                    b.Property<DateTimeOffset?>("CancelledOn")
                        .HasColumnType("datetime(6)");

                    b.Property<int>("Count")
                        .HasColumnType("int");

                    b.Property<byte[]>("FoodId")
                        .HasColumnType("binary(16)");

                    b.Property<byte[]>("InvoiceId")
                        .HasColumnType("binary(16)");

                    b.Property<int>("Status")
                        .HasColumnType("int");

                    b.Property<byte[]>("UserId")
                        .HasColumnType("binary(16)");

                    b.HasKey("Id");

                    b.HasIndex("FoodId");

                    b.HasIndex("InvoiceId");

                    b.HasIndex("UserId", "BookedOn");

                    b.ToTable("bookings", (string)null);
                });

            modelBuilder.Entity("GtKasse.Core.Entities.Food", b =>
                {
                    b.Property<byte[]>("Id")
                        .HasColumnType("binary(16)");

                    b.Property<byte[]>("FoodListId")
                        .HasColumnType("binary(16)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(128)
                        .HasColumnType("varchar(128)");

                    b.Property<decimal>("Price")
                        .HasColumnType("decimal(6,2)");

                    b.Property<int>("Type")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("FoodListId");

                    b.ToTable("foods", (string)null);
                });

            modelBuilder.Entity("GtKasse.Core.Entities.FoodList", b =>
                {
                    b.Property<byte[]>("Id")
                        .HasColumnType("binary(16)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(128)
                        .HasColumnType("varchar(128)");

                    b.Property<DateTimeOffset>("ValidFrom")
                        .HasColumnType("datetime(6)");

                    b.HasKey("Id");

                    b.HasIndex("ValidFrom");

                    b.ToTable("food_lists", (string)null);
                });

            modelBuilder.Entity("GtKasse.Core.Entities.Invoice", b =>
                {
                    b.Property<byte[]>("Id")
                        .HasColumnType("binary(16)");

                    b.Property<DateTimeOffset>("CreatedOn")
                        .HasColumnType("datetime(6)");

                    b.Property<DateTimeOffset?>("PaidOn")
                        .HasColumnType("datetime(6)");

                    b.Property<int>("Status")
                        .HasColumnType("int");

                    b.Property<decimal>("Total")
                        .HasColumnType("decimal(6,2)");

                    b.Property<byte[]>("UserId")
                        .HasColumnType("binary(16)");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("invoices", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.DataProtection.EntityFrameworkCore.DataProtectionKey", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("FriendlyName")
                        .HasColumnType("longtext");

                    b.Property<string>("Xml")
                        .HasColumnType("longtext");

                    b.HasKey("Id");

                    b.ToTable("data_protection_keys", (string)null);
                });

            modelBuilder.Entity("GtKasse.Core.Entities.AccountNotification", b =>
                {
                    b.HasOne("GtKasse.Core.Entities.IdentityUserGuid", "User")
                        .WithMany("AccountNotifications")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.NoAction);

                    b.Navigation("User");
                });

            modelBuilder.Entity("GtKasse.Core.Entities.IdentityRoleClaimGuid", b =>
                {
                    b.HasOne("GtKasse.Core.Entities.IdentityRoleGuid", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("GtKasse.Core.Entities.IdentityUserClaimGuid", b =>
                {
                    b.HasOne("GtKasse.Core.Entities.IdentityUserGuid", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("GtKasse.Core.Entities.IdentityUserLoginGuid", b =>
                {
                    b.HasOne("GtKasse.Core.Entities.IdentityUserGuid", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("GtKasse.Core.Entities.IdentityUserRoleGuid", b =>
                {
                    b.HasOne("GtKasse.Core.Entities.IdentityRoleGuid", "Role")
                        .WithMany("UserRoles")
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("GtKasse.Core.Entities.IdentityUserGuid", "User")
                        .WithMany("UserRoles")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Role");

                    b.Navigation("User");
                });

            modelBuilder.Entity("GtKasse.Core.Entities.IdentityUserTokenGuid", b =>
                {
                    b.HasOne("GtKasse.Core.Entities.IdentityUserGuid", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("GtKasse.Core.Entities.Booking", b =>
                {
                    b.HasOne("GtKasse.Core.Entities.Food", "Food")
                        .WithMany("Bookings")
                        .HasForeignKey("FoodId")
                        .OnDelete(DeleteBehavior.NoAction);

                    b.HasOne("GtKasse.Core.Entities.Invoice", "Invoice")
                        .WithMany("Bookings")
                        .HasForeignKey("InvoiceId")
                        .OnDelete(DeleteBehavior.NoAction);

                    b.HasOne("GtKasse.Core.Entities.IdentityUserGuid", "User")
                        .WithMany("Bookings")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.NoAction);

                    b.Navigation("Food");

                    b.Navigation("Invoice");

                    b.Navigation("User");
                });

            modelBuilder.Entity("GtKasse.Core.Entities.Food", b =>
                {
                    b.HasOne("GtKasse.Core.Entities.FoodList", "FoodList")
                        .WithMany("Foods")
                        .HasForeignKey("FoodListId")
                        .OnDelete(DeleteBehavior.NoAction);

                    b.Navigation("FoodList");
                });

            modelBuilder.Entity("GtKasse.Core.Entities.Invoice", b =>
                {
                    b.HasOne("GtKasse.Core.Entities.IdentityUserGuid", "User")
                        .WithMany()
                        .HasForeignKey("UserId");

                    b.Navigation("User");
                });

            modelBuilder.Entity("GtKasse.Core.Entities.IdentityRoleGuid", b =>
                {
                    b.Navigation("UserRoles");
                });

            modelBuilder.Entity("GtKasse.Core.Entities.IdentityUserGuid", b =>
                {
                    b.Navigation("AccountNotifications");

                    b.Navigation("Bookings");

                    b.Navigation("UserRoles");
                });

            modelBuilder.Entity("GtKasse.Core.Entities.Food", b =>
                {
                    b.Navigation("Bookings");
                });

            modelBuilder.Entity("GtKasse.Core.Entities.FoodList", b =>
                {
                    b.Navigation("Foods");
                });

            modelBuilder.Entity("GtKasse.Core.Entities.Invoice", b =>
                {
                    b.Navigation("Bookings");
                });
#pragma warning restore 612, 618
        }
    }
}
