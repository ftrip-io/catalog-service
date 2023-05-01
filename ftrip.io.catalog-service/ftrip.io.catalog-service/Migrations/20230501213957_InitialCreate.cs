using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ftrip.io.catalog_service.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AmenityTypes",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Active = table.Column<bool>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    ModifiedAt = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    ModifiedBy = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AmenityTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Locations",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Active = table.Column<bool>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    ModifiedAt = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    ModifiedBy = table.Column<string>(nullable: true),
                    Country = table.Column<string>(nullable: true),
                    Region = table.Column<string>(nullable: true),
                    City = table.Column<string>(nullable: true),
                    PostalCode = table.Column<string>(nullable: true),
                    Address = table.Column<string>(nullable: true),
                    Apt = table.Column<string>(nullable: true),
                    Latitude = table.Column<float>(nullable: false),
                    Longitude = table.Column<float>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Locations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PropertyTypes",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Active = table.Column<bool>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    ModifiedAt = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    ModifiedBy = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PropertyTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Amenities",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Active = table.Column<bool>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    ModifiedAt = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    ModifiedBy = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    Icon = table.Column<string>(nullable: true),
                    AmenityTypeId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Amenities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Amenities_AmenityTypes_AmenityTypeId",
                        column: x => x.AmenityTypeId,
                        principalTable: "AmenityTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Accommodations",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Active = table.Column<bool>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    ModifiedAt = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    ModifiedBy = table.Column<string>(nullable: true),
                    Title = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    PlaceType = table.Column<int>(nullable: false),
                    IsDecicatedForGuests = table.Column<bool>(nullable: false),
                    MinGuests = table.Column<int>(nullable: false),
                    MaxGuests = table.Column<int>(nullable: false),
                    BedroomCount = table.Column<int>(nullable: false),
                    BedCount = table.Column<int>(nullable: false),
                    BathroomCount = table.Column<int>(nullable: false),
                    NoticePeriod = table.Column<int>(nullable: false),
                    BookingAdvancePeriod = table.Column<int>(nullable: false),
                    CheckInFrom = table.Column<int>(nullable: false),
                    CheckInTo = table.Column<int>(nullable: false),
                    BookBeforeTime = table.Column<int>(nullable: false),
                    MinNights = table.Column<int>(nullable: false),
                    MaxNights = table.Column<int>(nullable: false),
                    Price = table.Column<decimal>(nullable: false),
                    IsPerGuest = table.Column<bool>(nullable: false),
                    HostId = table.Column<Guid>(nullable: false),
                    HouseRules = table.Column<string>(nullable: true),
                    LocationId = table.Column<Guid>(nullable: false),
                    PropertyTypeId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Accommodations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Accommodations_Locations_LocationId",
                        column: x => x.LocationId,
                        principalTable: "Locations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Accommodations_PropertyTypes_PropertyTypeId",
                        column: x => x.PropertyTypeId,
                        principalTable: "PropertyTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AccommodationAmenities",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Active = table.Column<bool>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    ModifiedAt = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    ModifiedBy = table.Column<string>(nullable: true),
                    AmenityId = table.Column<Guid>(nullable: false),
                    AccommodationId = table.Column<Guid>(nullable: false),
                    IsPresent = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccommodationAmenities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AccommodationAmenities_Accommodations_AccommodationId",
                        column: x => x.AccommodationId,
                        principalTable: "Accommodations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AccommodationAmenities_Amenities_AmenityId",
                        column: x => x.AmenityId,
                        principalTable: "Amenities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Availability",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Active = table.Column<bool>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    ModifiedAt = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    ModifiedBy = table.Column<string>(nullable: true),
                    FromDate = table.Column<DateTime>(nullable: false),
                    ToDate = table.Column<DateTime>(nullable: false),
                    IsAvailable = table.Column<bool>(nullable: false),
                    AccommodationId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Availability", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Availability_Accommodations_AccommodationId",
                        column: x => x.AccommodationId,
                        principalTable: "Accommodations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PriceDiff",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Active = table.Column<bool>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    ModifiedAt = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    ModifiedBy = table.Column<string>(nullable: true),
                    When = table.Column<string>(nullable: true),
                    Percentage = table.Column<float>(nullable: false),
                    AccommodationId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PriceDiff", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PriceDiff_Accommodations_AccommodationId",
                        column: x => x.AccommodationId,
                        principalTable: "Accommodations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AccommodationAmenities_AccommodationId",
                table: "AccommodationAmenities",
                column: "AccommodationId");

            migrationBuilder.CreateIndex(
                name: "IX_AccommodationAmenities_AmenityId",
                table: "AccommodationAmenities",
                column: "AmenityId");

            migrationBuilder.CreateIndex(
                name: "IX_Accommodations_LocationId",
                table: "Accommodations",
                column: "LocationId");

            migrationBuilder.CreateIndex(
                name: "IX_Accommodations_PropertyTypeId",
                table: "Accommodations",
                column: "PropertyTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Amenities_AmenityTypeId",
                table: "Amenities",
                column: "AmenityTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Availability_AccommodationId",
                table: "Availability",
                column: "AccommodationId");

            migrationBuilder.CreateIndex(
                name: "IX_PriceDiff_AccommodationId",
                table: "PriceDiff",
                column: "AccommodationId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AccommodationAmenities");

            migrationBuilder.DropTable(
                name: "Availability");

            migrationBuilder.DropTable(
                name: "PriceDiff");

            migrationBuilder.DropTable(
                name: "Amenities");

            migrationBuilder.DropTable(
                name: "Accommodations");

            migrationBuilder.DropTable(
                name: "AmenityTypes");

            migrationBuilder.DropTable(
                name: "Locations");

            migrationBuilder.DropTable(
                name: "PropertyTypes");
        }
    }
}
