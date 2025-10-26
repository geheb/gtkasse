﻿using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace GtKasse.Core.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "boats",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", maxLength: 32, nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 64, nullable: false),
                    Identifier = table.Column<string>(type: "TEXT", maxLength: 8, nullable: false),
                    Location = table.Column<string>(type: "TEXT", maxLength: 64, nullable: false),
                    MaxRentalDays = table.Column<int>(type: "INTEGER", nullable: false),
                    IsLocked = table.Column<bool>(type: "INTEGER", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_boats", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "clubhouse_bookings",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", maxLength: 32, nullable: false),
                    Start = table.Column<DateTime>(type: "TEXT", nullable: false),
                    End = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Title = table.Column<string>(type: "TEXT", maxLength: 256, nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_clubhouse_bookings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "email_queue",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", maxLength: 32, nullable: false),
                    Created = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Updated = table.Column<DateTime>(type: "TEXT", nullable: true),
                    Sent = table.Column<DateTime>(type: "TEXT", nullable: true),
                    Recipient = table.Column<string>(type: "TEXT", nullable: false),
                    Subject = table.Column<string>(type: "TEXT", nullable: false),
                    HtmlBody = table.Column<string>(type: "TEXT", nullable: false),
                    IsPrio = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_email_queue", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "food_lists",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", maxLength: 32, nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    ValidFrom = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_food_lists", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "identity_roles",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", maxLength: 32, nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_identity_roles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "identity_users",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", maxLength: 32, nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    LastLogin = table.Column<DateTime>(type: "TEXT", nullable: true),
                    LeftOn = table.Column<DateTime>(type: "TEXT", nullable: true),
                    DebtorNumber = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    AddressNumber = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    AuthenticatorKey = table.Column<string>(type: "TEXT", nullable: true),
                    UserName = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "INTEGER", nullable: false),
                    PasswordHash = table.Column<string>(type: "TEXT", nullable: true),
                    SecurityStamp = table.Column<string>(type: "TEXT", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "TEXT", nullable: true),
                    PhoneNumber = table.Column<string>(type: "TEXT", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "INTEGER", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "INTEGER", nullable: false),
                    LockoutEnd = table.Column<DateTime>(type: "TEXT", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "INTEGER", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_identity_users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "invoice_periods",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", maxLength: 32, nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: false),
                    From = table.Column<DateTime>(type: "TEXT", nullable: false),
                    To = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_invoice_periods", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "mailings",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", maxLength: 32, nullable: false),
                    Created = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Updated = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CanSendToAllMembers = table.Column<bool>(type: "INTEGER", nullable: false),
                    OtherRecipients = table.Column<string>(type: "TEXT", nullable: true),
                    ReplyAddress = table.Column<string>(type: "TEXT", nullable: true),
                    Subject = table.Column<string>(type: "TEXT", nullable: false),
                    HtmlBody = table.Column<string>(type: "TEXT", nullable: false),
                    IsClosed = table.Column<bool>(type: "INTEGER", nullable: false),
                    EmailCount = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_mailings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "vehicles",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", maxLength: 32, nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 64, nullable: false),
                    Identifier = table.Column<string>(type: "TEXT", maxLength: 12, nullable: false),
                    IsInUse = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_vehicles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "foods",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", maxLength: 32, nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    Price = table.Column<decimal>(type: "TEXT", precision: 6, scale: 2, nullable: false),
                    Type = table.Column<int>(type: "INTEGER", nullable: false),
                    FoodListId = table.Column<string>(type: "TEXT", maxLength: 32, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_foods", x => x.Id);
                    table.ForeignKey(
                        name: "FK_foods_food_lists_FoodListId",
                        column: x => x.FoodListId,
                        principalTable: "food_lists",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "identity_role_claims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false),
                    RoleId = table.Column<string>(type: "TEXT", maxLength: 32, nullable: false),
                    ClaimType = table.Column<string>(type: "TEXT", nullable: true),
                    ClaimValue = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_identity_role_claims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_identity_role_claims_identity_roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "identity_roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "boat_rentals",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", maxLength: 32, nullable: false),
                    BoatId = table.Column<string>(type: "TEXT", maxLength: 32, nullable: true),
                    UserId = table.Column<string>(type: "TEXT", maxLength: 32, nullable: true),
                    Start = table.Column<DateTime>(type: "TEXT", nullable: false),
                    End = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Purpose = table.Column<string>(type: "TEXT", maxLength: 256, nullable: false),
                    CancelledOn = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_boat_rentals", x => x.Id);
                    table.ForeignKey(
                        name: "FK_boat_rentals_boats_BoatId",
                        column: x => x.BoatId,
                        principalTable: "boats",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_boat_rentals_identity_users_UserId",
                        column: x => x.UserId,
                        principalTable: "identity_users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "identity_user_claims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UserId = table.Column<string>(type: "TEXT", maxLength: 32, nullable: false),
                    ClaimType = table.Column<string>(type: "TEXT", nullable: true),
                    ClaimValue = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_identity_user_claims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_identity_user_claims_identity_users_UserId",
                        column: x => x.UserId,
                        principalTable: "identity_users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "identity_user_logins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "TEXT", nullable: false),
                    ProviderKey = table.Column<string>(type: "TEXT", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "TEXT", nullable: true),
                    UserId = table.Column<string>(type: "TEXT", maxLength: 32, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_identity_user_logins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_identity_user_logins_identity_users_UserId",
                        column: x => x.UserId,
                        principalTable: "identity_users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "identity_user_roles",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "TEXT", maxLength: 32, nullable: false),
                    RoleId = table.Column<string>(type: "TEXT", maxLength: 32, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_identity_user_roles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_identity_user_roles_identity_roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "identity_roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_identity_user_roles_identity_users_UserId",
                        column: x => x.UserId,
                        principalTable: "identity_users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "identity_user_tokens",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "TEXT", maxLength: 32, nullable: false),
                    LoginProvider = table.Column<string>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Value = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_identity_user_tokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_identity_user_tokens_identity_users_UserId",
                        column: x => x.UserId,
                        principalTable: "identity_users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "trips",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", maxLength: 32, nullable: false),
                    Start = table.Column<DateTime>(type: "TEXT", nullable: false),
                    End = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UserId = table.Column<string>(type: "TEXT", maxLength: 32, nullable: true),
                    Target = table.Column<string>(type: "TEXT", nullable: false),
                    MaxBookings = table.Column<int>(type: "INTEGER", nullable: false),
                    BookingStart = table.Column<DateTime>(type: "TEXT", nullable: false),
                    BookingEnd = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: true),
                    Categories = table.Column<int>(type: "INTEGER", nullable: false),
                    IsPublic = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_trips", x => x.Id);
                    table.ForeignKey(
                        name: "FK_trips_identity_users_UserId",
                        column: x => x.UserId,
                        principalTable: "identity_users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "tryouts",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", maxLength: 32, nullable: false),
                    Type = table.Column<string>(type: "TEXT", maxLength: 64, nullable: false),
                    Date = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UserId = table.Column<string>(type: "TEXT", maxLength: 32, nullable: true),
                    MaxBookings = table.Column<int>(type: "INTEGER", nullable: false),
                    BookingStart = table.Column<DateTime>(type: "TEXT", nullable: false),
                    BookingEnd = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tryouts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_tryouts_identity_users_UserId",
                        column: x => x.UserId,
                        principalTable: "identity_users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "wiki_articles",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", maxLength: 32, nullable: false),
                    Created = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Updated = table.Column<DateTime>(type: "TEXT", nullable: true),
                    UserId = table.Column<string>(type: "TEXT", maxLength: 32, nullable: true),
                    Identifier = table.Column<string>(type: "TEXT", maxLength: 16, nullable: false),
                    Title = table.Column<string>(type: "TEXT", maxLength: 256, nullable: false),
                    Content = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_wiki_articles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_wiki_articles_identity_users_UserId",
                        column: x => x.UserId,
                        principalTable: "identity_users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "invoices",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", maxLength: 32, nullable: false),
                    UserId = table.Column<string>(type: "TEXT", maxLength: 32, nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Total = table.Column<decimal>(type: "TEXT", precision: 6, scale: 2, nullable: false),
                    Status = table.Column<int>(type: "INTEGER", nullable: false),
                    PaidOn = table.Column<DateTime>(type: "TEXT", nullable: true),
                    InvoicePeriodId = table.Column<string>(type: "TEXT", maxLength: 32, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_invoices", x => x.Id);
                    table.ForeignKey(
                        name: "FK_invoices_identity_users_UserId",
                        column: x => x.UserId,
                        principalTable: "identity_users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_invoices_invoice_periods_InvoicePeriodId",
                        column: x => x.InvoicePeriodId,
                        principalTable: "invoice_periods",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "my_mailings",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", maxLength: 32, nullable: false),
                    Created = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Updated = table.Column<DateTime>(type: "TEXT", nullable: true),
                    MailingId = table.Column<string>(type: "TEXT", maxLength: 32, nullable: true),
                    UserId = table.Column<string>(type: "TEXT", maxLength: 32, nullable: true),
                    HasRead = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_my_mailings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_my_mailings_identity_users_UserId",
                        column: x => x.UserId,
                        principalTable: "identity_users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_my_mailings_mailings_MailingId",
                        column: x => x.MailingId,
                        principalTable: "mailings",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "vehicle_bookings",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", maxLength: 32, nullable: false),
                    VehicleId = table.Column<string>(type: "TEXT", maxLength: 32, nullable: true),
                    UserId = table.Column<string>(type: "TEXT", maxLength: 32, nullable: true),
                    Start = table.Column<DateTime>(type: "TEXT", nullable: false),
                    End = table.Column<DateTime>(type: "TEXT", nullable: false),
                    BookedOn = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ConfirmedOn = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CancelledOn = table.Column<DateTime>(type: "TEXT", nullable: true),
                    Purpose = table.Column<string>(type: "TEXT", maxLength: 256, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_vehicle_bookings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_vehicle_bookings_identity_users_UserId",
                        column: x => x.UserId,
                        principalTable: "identity_users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_vehicle_bookings_vehicles_VehicleId",
                        column: x => x.VehicleId,
                        principalTable: "vehicles",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "trip_bookings",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", maxLength: 32, nullable: false),
                    TripId = table.Column<string>(type: "TEXT", maxLength: 32, nullable: true),
                    UserId = table.Column<string>(type: "TEXT", maxLength: 32, nullable: true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    BookedOn = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ConfirmedOn = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CancelledOn = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_trip_bookings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_trip_bookings_identity_users_UserId",
                        column: x => x.UserId,
                        principalTable: "identity_users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_trip_bookings_trips_TripId",
                        column: x => x.TripId,
                        principalTable: "trips",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "trip_chats",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", maxLength: 32, nullable: false),
                    TripId = table.Column<string>(type: "TEXT", maxLength: 32, nullable: true),
                    UserId = table.Column<string>(type: "TEXT", maxLength: 32, nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Message = table.Column<string>(type: "TEXT", maxLength: 256, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_trip_chats", x => x.Id);
                    table.ForeignKey(
                        name: "FK_trip_chats_identity_users_UserId",
                        column: x => x.UserId,
                        principalTable: "identity_users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_trip_chats_trips_TripId",
                        column: x => x.TripId,
                        principalTable: "trips",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "tryout_bookings",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", maxLength: 32, nullable: false),
                    TryoutId = table.Column<string>(type: "TEXT", maxLength: 32, nullable: true),
                    UserId = table.Column<string>(type: "TEXT", maxLength: 32, nullable: true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    BookedOn = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ConfirmedOn = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CancelledOn = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tryout_bookings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_tryout_bookings_identity_users_UserId",
                        column: x => x.UserId,
                        principalTable: "identity_users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_tryout_bookings_tryouts_TryoutId",
                        column: x => x.TryoutId,
                        principalTable: "tryouts",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "tryout_chats",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", maxLength: 32, nullable: false),
                    TryoutId = table.Column<string>(type: "TEXT", maxLength: 32, nullable: true),
                    UserId = table.Column<string>(type: "TEXT", maxLength: 32, nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Message = table.Column<string>(type: "TEXT", maxLength: 256, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tryout_chats", x => x.Id);
                    table.ForeignKey(
                        name: "FK_tryout_chats_identity_users_UserId",
                        column: x => x.UserId,
                        principalTable: "identity_users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_tryout_chats_tryouts_TryoutId",
                        column: x => x.TryoutId,
                        principalTable: "tryouts",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "food_bookings",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", maxLength: 32, nullable: false),
                    UserId = table.Column<string>(type: "TEXT", maxLength: 32, nullable: true),
                    FoodId = table.Column<string>(type: "TEXT", maxLength: 32, nullable: true),
                    Status = table.Column<int>(type: "INTEGER", nullable: false),
                    Count = table.Column<int>(type: "INTEGER", nullable: false),
                    BookedOn = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CancelledOn = table.Column<DateTime>(type: "TEXT", nullable: true),
                    InvoiceId = table.Column<string>(type: "TEXT", maxLength: 32, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_food_bookings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_food_bookings_foods_FoodId",
                        column: x => x.FoodId,
                        principalTable: "foods",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_food_bookings_identity_users_UserId",
                        column: x => x.UserId,
                        principalTable: "identity_users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_food_bookings_invoices_InvoiceId",
                        column: x => x.InvoiceId,
                        principalTable: "invoices",
                        principalColumn: "Id");
                });

            migrationBuilder.InsertData(
                table: "identity_roles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "02c2dfe3733145f6929bf983273f2d74", "4B04648D-DE82-4B0B-B014-3CE0BE5454FD", "wikimanager", "WIKIMANAGER" },
                    { "03a90da5abf045e99b80cdac790b7105", "956824FE-2F13-4919-B2D2-0E60BECFCA12", "member", "MEMBER" },
                    { "1d4d07e040ae45cba4f6397672f3605b", "995137CB-BD7B-4747-884E-A7467F3C0A3A", "kitchen", "KITCHEN" },
                    { "1fb645763f904ce98c3acfa13f587167", "6EE38FDC-5CE5-42FD-A5A5-168573DB2F86", "treasurer", "TREASURER" },
                    { "2ddc9beceb964cd594b4dd0d99a19664", "F3BFC39C-70B7-4E91-A70B-131925290446", "tripmanager", "TRIPMANAGER" },
                    { "668ef80a9f574961beac540ad8003241", "4B04648D-DE82-4B0B-B014-3CE0BE5454FD", "boatmanager", "BOATMANAGER" },
                    { "6dc9442b327946cc95c01d4f8c3d31f0", "4B04648D-DE82-4B0B-B014-3CE0BE5454FD", "mailingmanager", "MAILINGMANAGER" },
                    { "8772da16fbbc46afa52cc83c90ca74c9", "49DD2CBF-AAC9-4015-9016-9E3ED25547DF", "chairperson", "CHAIRPERSON" },
                    { "9dc6f0a7711648208b764f20b73053ee", "3096E774-529C-41B4-8CD3-355E0D2C930D", "interested", "INTERESTED" },
                    { "afd40e75dcfe4c469d4cff701f225179", "4B04648D-DE82-4B0B-B014-3CE0BE5454FD", "housemanager", "HOUSEMANAGER" },
                    { "ef80d8424a0f461382eb48a670bac21d", "EA00C945-985C-42AD-B149-7D6FBCE9C279", "usermanager", "USERMANAGER" },
                    { "f949306030b84bf3a4be74630b0fa423", "D8FEF733-1C63-433F-916F-99B8645D1487", "fleetmanager", "FLEETMANAGER" },
                    { "fbbcf7bf8cbf440f939cf67631109aa0", "CFAD6F62-EEAA-4ECD-B847-0762C704EC45", "administrator", "ADMINISTRATOR" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_boat_rentals_BoatId",
                table: "boat_rentals",
                column: "BoatId");

            migrationBuilder.CreateIndex(
                name: "IX_boat_rentals_UserId",
                table: "boat_rentals",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_email_queue_Sent_Created_IsPrio",
                table: "email_queue",
                columns: new[] { "Sent", "Created", "IsPrio" });

            migrationBuilder.CreateIndex(
                name: "IX_food_bookings_FoodId",
                table: "food_bookings",
                column: "FoodId");

            migrationBuilder.CreateIndex(
                name: "IX_food_bookings_InvoiceId",
                table: "food_bookings",
                column: "InvoiceId");

            migrationBuilder.CreateIndex(
                name: "IX_food_bookings_UserId_BookedOn",
                table: "food_bookings",
                columns: new[] { "UserId", "BookedOn" });

            migrationBuilder.CreateIndex(
                name: "IX_foods_FoodListId",
                table: "foods",
                column: "FoodListId");

            migrationBuilder.CreateIndex(
                name: "IX_identity_role_claims_RoleId",
                table: "identity_role_claims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_identity_roles_NormalizedName",
                table: "identity_roles",
                column: "NormalizedName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_identity_user_claims_UserId",
                table: "identity_user_claims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_identity_user_logins_UserId",
                table: "identity_user_logins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_identity_user_roles_RoleId",
                table: "identity_user_roles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_identity_users_NormalizedEmail",
                table: "identity_users",
                column: "NormalizedEmail",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_identity_users_NormalizedUserName",
                table: "identity_users",
                column: "NormalizedUserName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_invoice_periods_To",
                table: "invoice_periods",
                column: "To");

            migrationBuilder.CreateIndex(
                name: "IX_invoices_InvoicePeriodId",
                table: "invoices",
                column: "InvoicePeriodId");

            migrationBuilder.CreateIndex(
                name: "IX_invoices_UserId",
                table: "invoices",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_my_mailings_MailingId",
                table: "my_mailings",
                column: "MailingId");

            migrationBuilder.CreateIndex(
                name: "IX_my_mailings_UserId",
                table: "my_mailings",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_trip_bookings_TripId",
                table: "trip_bookings",
                column: "TripId");

            migrationBuilder.CreateIndex(
                name: "IX_trip_bookings_UserId",
                table: "trip_bookings",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_trip_chats_TripId",
                table: "trip_chats",
                column: "TripId");

            migrationBuilder.CreateIndex(
                name: "IX_trip_chats_UserId",
                table: "trip_chats",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_trips_Start_End",
                table: "trips",
                columns: new[] { "Start", "End" });

            migrationBuilder.CreateIndex(
                name: "IX_trips_UserId",
                table: "trips",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_tryout_bookings_TryoutId",
                table: "tryout_bookings",
                column: "TryoutId");

            migrationBuilder.CreateIndex(
                name: "IX_tryout_bookings_UserId",
                table: "tryout_bookings",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_tryout_chats_TryoutId",
                table: "tryout_chats",
                column: "TryoutId");

            migrationBuilder.CreateIndex(
                name: "IX_tryout_chats_UserId",
                table: "tryout_chats",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_tryouts_UserId",
                table: "tryouts",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_vehicle_bookings_UserId",
                table: "vehicle_bookings",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_vehicle_bookings_VehicleId",
                table: "vehicle_bookings",
                column: "VehicleId");

            migrationBuilder.CreateIndex(
                name: "IX_wiki_articles_Identifier",
                table: "wiki_articles",
                column: "Identifier",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_wiki_articles_UserId",
                table: "wiki_articles",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "boat_rentals");

            migrationBuilder.DropTable(
                name: "clubhouse_bookings");

            migrationBuilder.DropTable(
                name: "email_queue");

            migrationBuilder.DropTable(
                name: "food_bookings");

            migrationBuilder.DropTable(
                name: "identity_role_claims");

            migrationBuilder.DropTable(
                name: "identity_user_claims");

            migrationBuilder.DropTable(
                name: "identity_user_logins");

            migrationBuilder.DropTable(
                name: "identity_user_roles");

            migrationBuilder.DropTable(
                name: "identity_user_tokens");

            migrationBuilder.DropTable(
                name: "my_mailings");

            migrationBuilder.DropTable(
                name: "trip_bookings");

            migrationBuilder.DropTable(
                name: "trip_chats");

            migrationBuilder.DropTable(
                name: "tryout_bookings");

            migrationBuilder.DropTable(
                name: "tryout_chats");

            migrationBuilder.DropTable(
                name: "vehicle_bookings");

            migrationBuilder.DropTable(
                name: "wiki_articles");

            migrationBuilder.DropTable(
                name: "boats");

            migrationBuilder.DropTable(
                name: "foods");

            migrationBuilder.DropTable(
                name: "invoices");

            migrationBuilder.DropTable(
                name: "identity_roles");

            migrationBuilder.DropTable(
                name: "mailings");

            migrationBuilder.DropTable(
                name: "trips");

            migrationBuilder.DropTable(
                name: "tryouts");

            migrationBuilder.DropTable(
                name: "vehicles");

            migrationBuilder.DropTable(
                name: "food_lists");

            migrationBuilder.DropTable(
                name: "invoice_periods");

            migrationBuilder.DropTable(
                name: "identity_users");
        }
    }
}
