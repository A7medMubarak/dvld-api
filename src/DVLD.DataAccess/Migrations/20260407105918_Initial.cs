using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DVLD.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ApplicationTypes",
                columns: table => new
                {
                    ApplicationTypeId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Fees = table.Column<decimal>(type: "decimal(6,2)", precision: 6, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApplicationTypes", x => x.ApplicationTypeId);
                });

            migrationBuilder.CreateTable(
                name: "Countries",
                columns: table => new
                {
                    CountryId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CountryName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Countries", x => x.CountryId);
                });

            migrationBuilder.CreateTable(
                name: "LicenseClasses",
                columns: table => new
                {
                    LicenseClassId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ClassName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ClassDescription = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    MinimumAllowedAge = table.Column<byte>(type: "TINYINT", nullable: false),
                    DefaultValidityLength = table.Column<byte>(type: "TINYINT", nullable: false),
                    ClassFees = table.Column<decimal>(type: "decimal(6,2)", precision: 6, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LicenseClasses", x => x.LicenseClassId);
                });

            migrationBuilder.CreateTable(
                name: "TestTypes",
                columns: table => new
                {
                    TestTypeId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Fees = table.Column<decimal>(type: "decimal(6,2)", precision: 6, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TestTypes", x => x.TestTypeId);
                });

            migrationBuilder.CreateTable(
                name: "People",
                columns: table => new
                {
                    PersonId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NationalNo = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    SecondName = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    ThirdName = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    LastName = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    DateOfBirth = table.Column<DateTime>(type: "DATETIME", nullable: false),
                    Address = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Gender = table.Column<byte>(type: "tinyint", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    NationalityCountryId = table.Column<int>(type: "int", nullable: false),
                    ImagePath = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_People", x => x.PersonId);
                    table.ForeignKey(
                        name: "FK_People_Countries_NationalityCountryId",
                        column: x => x.NationalityCountryId,
                        principalTable: "Countries",
                        principalColumn: "CountryId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PersonId = table.Column<int>(type: "int", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    IsActive = table.Column<bool>(type: "BIT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.UserId);
                    table.ForeignKey(
                        name: "FK_Users_People_PersonId",
                        column: x => x.PersonId,
                        principalTable: "People",
                        principalColumn: "PersonId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Applications",
                columns: table => new
                {
                    ApplicationId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ApplicantPersonId = table.Column<int>(type: "int", nullable: false),
                    ApplicationDate = table.Column<DateTime>(type: "DATETIME", nullable: false),
                    ApplicationTypeId = table.Column<int>(type: "int", nullable: false),
                    ApplicationStatus = table.Column<byte>(type: "tinyint", nullable: false),
                    LastStatusDate = table.Column<DateTime>(type: "DATETIME", nullable: false),
                    PaidFees = table.Column<decimal>(type: "decimal(6,2)", precision: 6, scale: 2, nullable: false),
                    CreatedByUserId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Applications", x => x.ApplicationId);
                    table.ForeignKey(
                        name: "FK_Applications_ApplicationTypes_ApplicationTypeId",
                        column: x => x.ApplicationTypeId,
                        principalTable: "ApplicationTypes",
                        principalColumn: "ApplicationTypeId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Applications_People_ApplicantPersonId",
                        column: x => x.ApplicantPersonId,
                        principalTable: "People",
                        principalColumn: "PersonId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Applications_Users_CreatedByUserId",
                        column: x => x.CreatedByUserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Drivers",
                columns: table => new
                {
                    DriverId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PersonId = table.Column<int>(type: "int", nullable: false),
                    CreatedByUserId = table.Column<int>(type: "int", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "DATETIME", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Drivers", x => x.DriverId);
                    table.ForeignKey(
                        name: "FK_Drivers_People_PersonId",
                        column: x => x.PersonId,
                        principalTable: "People",
                        principalColumn: "PersonId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Drivers_Users_CreatedByUserId",
                        column: x => x.CreatedByUserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "LocalDrivingLicenseApplications",
                columns: table => new
                {
                    LocalDrivingLicenseApplicationId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ApplicationId = table.Column<int>(type: "int", nullable: false),
                    LicenseClassId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LocalDrivingLicenseApplications", x => x.LocalDrivingLicenseApplicationId);
                    table.ForeignKey(
                        name: "FK_LocalDrivingLicenseApplications_Applications_ApplicationId",
                        column: x => x.ApplicationId,
                        principalTable: "Applications",
                        principalColumn: "ApplicationId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LocalDrivingLicenseApplications_LicenseClasses_LicenseClassId",
                        column: x => x.LicenseClassId,
                        principalTable: "LicenseClasses",
                        principalColumn: "LicenseClassId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Licenses",
                columns: table => new
                {
                    LicenseId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LicenseClassId = table.Column<int>(type: "int", nullable: false),
                    ApplicationId = table.Column<int>(type: "int", nullable: false),
                    DriverId = table.Column<int>(type: "int", nullable: false),
                    IssueDate = table.Column<DateTime>(type: "DATETIME", nullable: false),
                    ExpirationDate = table.Column<DateTime>(type: "DATETIME", nullable: false),
                    IssueReason = table.Column<byte>(type: "tinyint", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    PaidFees = table.Column<decimal>(type: "decimal(6,2)", precision: 6, scale: 2, nullable: false),
                    IsActive = table.Column<bool>(type: "BIT", nullable: false),
                    CreatedByUserId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Licenses", x => x.LicenseId);
                    table.ForeignKey(
                        name: "FK_Licenses_Applications_ApplicationId",
                        column: x => x.ApplicationId,
                        principalTable: "Applications",
                        principalColumn: "ApplicationId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Licenses_Drivers_DriverId",
                        column: x => x.DriverId,
                        principalTable: "Drivers",
                        principalColumn: "DriverId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Licenses_LicenseClasses_LicenseClassId",
                        column: x => x.LicenseClassId,
                        principalTable: "LicenseClasses",
                        principalColumn: "LicenseClassId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Licenses_Users_CreatedByUserId",
                        column: x => x.CreatedByUserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "InternationalLicenses",
                columns: table => new
                {
                    InternationalLicenseId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ApplicationId = table.Column<int>(type: "int", nullable: false),
                    DriverId = table.Column<int>(type: "int", nullable: false),
                    IssuedUsingLocalLicenseId = table.Column<int>(type: "int", nullable: false),
                    IssueDate = table.Column<DateTime>(type: "DATETIME", nullable: false),
                    ExpirationDate = table.Column<DateTime>(type: "DATETIME", nullable: false),
                    IsActive = table.Column<bool>(type: "BIT", nullable: false),
                    CreatedByUserId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InternationalLicenses", x => x.InternationalLicenseId);
                    table.ForeignKey(
                        name: "FK_InternationalLicenses_Applications_ApplicationId",
                        column: x => x.ApplicationId,
                        principalTable: "Applications",
                        principalColumn: "ApplicationId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InternationalLicenses_Drivers_DriverId",
                        column: x => x.DriverId,
                        principalTable: "Drivers",
                        principalColumn: "DriverId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InternationalLicenses_LocalDrivingLicenseApplications_IssuedUsingLocalLicenseId",
                        column: x => x.IssuedUsingLocalLicenseId,
                        principalTable: "LocalDrivingLicenseApplications",
                        principalColumn: "LocalDrivingLicenseApplicationId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InternationalLicenses_Users_CreatedByUserId",
                        column: x => x.CreatedByUserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TestAppointments",
                columns: table => new
                {
                    TestAppointmentId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TestTypeId = table.Column<int>(type: "int", nullable: false),
                    LocalDrivingLicenseApplicationId = table.Column<int>(type: "int", nullable: false),
                    AppointmentDate = table.Column<DateTime>(type: "DATETIME", nullable: false),
                    PaidFees = table.Column<decimal>(type: "decimal(6,2)", precision: 6, scale: 2, nullable: false),
                    CreatedByUserId = table.Column<int>(type: "int", nullable: false),
                    IsLocked = table.Column<bool>(type: "BIT", nullable: false),
                    RetakeTestApplicationId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TestAppointments", x => x.TestAppointmentId);
                    table.ForeignKey(
                        name: "FK_TestAppointments_Applications_RetakeTestApplicationId",
                        column: x => x.RetakeTestApplicationId,
                        principalTable: "Applications",
                        principalColumn: "ApplicationId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TestAppointments_LocalDrivingLicenseApplications_LocalDrivingLicenseApplicationId",
                        column: x => x.LocalDrivingLicenseApplicationId,
                        principalTable: "LocalDrivingLicenseApplications",
                        principalColumn: "LocalDrivingLicenseApplicationId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TestAppointments_TestTypes_TestTypeId",
                        column: x => x.TestTypeId,
                        principalTable: "TestTypes",
                        principalColumn: "TestTypeId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TestAppointments_Users_CreatedByUserId",
                        column: x => x.CreatedByUserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DetainedLicenses",
                columns: table => new
                {
                    DetainId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LicenseId = table.Column<int>(type: "int", nullable: false),
                    DetainDate = table.Column<DateTime>(type: "DATETIME", nullable: false),
                    FineFees = table.Column<decimal>(type: "decimal(6,2)", precision: 6, scale: 2, nullable: false),
                    CreatedByUserId = table.Column<int>(type: "int", nullable: false),
                    IsReleased = table.Column<bool>(type: "BIT", nullable: false),
                    ReleaseDate = table.Column<DateTime>(type: "DATETIME", nullable: true),
                    ReleaseByUserId = table.Column<int>(type: "int", nullable: true),
                    ReleaseApplicationId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DetainedLicenses", x => x.DetainId);
                    table.ForeignKey(
                        name: "FK_DetainedLicenses_Applications_ReleaseApplicationId",
                        column: x => x.ReleaseApplicationId,
                        principalTable: "Applications",
                        principalColumn: "ApplicationId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DetainedLicenses_Licenses_LicenseId",
                        column: x => x.LicenseId,
                        principalTable: "Licenses",
                        principalColumn: "LicenseId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DetainedLicenses_Users_CreatedByUserId",
                        column: x => x.CreatedByUserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DetainedLicenses_Users_ReleaseByUserId",
                        column: x => x.ReleaseByUserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Tests",
                columns: table => new
                {
                    TestId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TestAppointmentId = table.Column<int>(type: "int", nullable: false),
                    TestResult = table.Column<bool>(type: "BIT", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedByUserId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tests", x => x.TestId);
                    table.ForeignKey(
                        name: "FK_Tests_TestAppointments_TestAppointmentId",
                        column: x => x.TestAppointmentId,
                        principalTable: "TestAppointments",
                        principalColumn: "TestAppointmentId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Tests_Users_CreatedByUserId",
                        column: x => x.CreatedByUserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Applications_ApplicantPersonId",
                table: "Applications",
                column: "ApplicantPersonId");

            migrationBuilder.CreateIndex(
                name: "IX_Applications_ApplicationTypeId",
                table: "Applications",
                column: "ApplicationTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Applications_CreatedByUserId",
                table: "Applications",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_DetainedLicenses_CreatedByUserId",
                table: "DetainedLicenses",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_DetainedLicenses_LicenseId",
                table: "DetainedLicenses",
                column: "LicenseId");

            migrationBuilder.CreateIndex(
                name: "IX_DetainedLicenses_ReleaseApplicationId",
                table: "DetainedLicenses",
                column: "ReleaseApplicationId",
                unique: true,
                filter: "[ReleaseApplicationId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_DetainedLicenses_ReleaseByUserId",
                table: "DetainedLicenses",
                column: "ReleaseByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Drivers_CreatedByUserId",
                table: "Drivers",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Drivers_PersonId",
                table: "Drivers",
                column: "PersonId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_InternationalLicenses_ApplicationId",
                table: "InternationalLicenses",
                column: "ApplicationId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_InternationalLicenses_CreatedByUserId",
                table: "InternationalLicenses",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_InternationalLicenses_DriverId",
                table: "InternationalLicenses",
                column: "DriverId");

            migrationBuilder.CreateIndex(
                name: "IX_InternationalLicenses_IssuedUsingLocalLicenseId",
                table: "InternationalLicenses",
                column: "IssuedUsingLocalLicenseId");

            migrationBuilder.CreateIndex(
                name: "IX_Licenses_ApplicationId",
                table: "Licenses",
                column: "ApplicationId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Licenses_CreatedByUserId",
                table: "Licenses",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Licenses_DriverId",
                table: "Licenses",
                column: "DriverId");

            migrationBuilder.CreateIndex(
                name: "IX_Licenses_LicenseClassId",
                table: "Licenses",
                column: "LicenseClassId");

            migrationBuilder.CreateIndex(
                name: "IX_LocalDrivingLicenseApplications_ApplicationId",
                table: "LocalDrivingLicenseApplications",
                column: "ApplicationId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LocalDrivingLicenseApplications_LicenseClassId",
                table: "LocalDrivingLicenseApplications",
                column: "LicenseClassId");

            migrationBuilder.CreateIndex(
                name: "IX_People_NationalityCountryId",
                table: "People",
                column: "NationalityCountryId");

            migrationBuilder.CreateIndex(
                name: "IX_People_NationalNo",
                table: "People",
                column: "NationalNo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TestAppointments_CreatedByUserId",
                table: "TestAppointments",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_TestAppointments_LocalDrivingLicenseApplicationId",
                table: "TestAppointments",
                column: "LocalDrivingLicenseApplicationId");

            migrationBuilder.CreateIndex(
                name: "IX_TestAppointments_RetakeTestApplicationId",
                table: "TestAppointments",
                column: "RetakeTestApplicationId",
                unique: true,
                filter: "[RetakeTestApplicationId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_TestAppointments_TestTypeId",
                table: "TestAppointments",
                column: "TestTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Tests_CreatedByUserId",
                table: "Tests",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Tests_TestAppointmentId",
                table: "Tests",
                column: "TestAppointmentId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_PersonId",
                table: "Users",
                column: "PersonId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_UserName",
                table: "Users",
                column: "UserName",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DetainedLicenses");

            migrationBuilder.DropTable(
                name: "InternationalLicenses");

            migrationBuilder.DropTable(
                name: "Tests");

            migrationBuilder.DropTable(
                name: "Licenses");

            migrationBuilder.DropTable(
                name: "TestAppointments");

            migrationBuilder.DropTable(
                name: "Drivers");

            migrationBuilder.DropTable(
                name: "LocalDrivingLicenseApplications");

            migrationBuilder.DropTable(
                name: "TestTypes");

            migrationBuilder.DropTable(
                name: "Applications");

            migrationBuilder.DropTable(
                name: "LicenseClasses");

            migrationBuilder.DropTable(
                name: "ApplicationTypes");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "People");

            migrationBuilder.DropTable(
                name: "Countries");
        }
    }
}
