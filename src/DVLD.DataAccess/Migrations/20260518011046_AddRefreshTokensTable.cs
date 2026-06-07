using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DVLD.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddRefreshTokensTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_People_Users_CreatedByUserId",
                table: "People");

            migrationBuilder.RenameColumn(
                name: "CreatedByUserId",
                table: "People",
                newName: "CreatedByUserId");

            migrationBuilder.RenameIndex(
                name: "IX_People_CreatedByUserId",
                table: "People",
                newName: "IX_People_CreatedByUserId");

            migrationBuilder.CreateTable(
                name: "RefreshTokens",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    Token = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RevokedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReplacedByToken = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RefreshTokens", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RefreshTokens_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RefreshTokens_Token",
                table: "RefreshTokens",
                column: "Token",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RefreshTokens_UserId",
                table: "RefreshTokens",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_People_Users_CreatedByUserId",
                table: "People",
                column: "CreatedByUserId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_People_Users_CreatedByUserId",
                table: "People");

            migrationBuilder.DropTable(
                name: "RefreshTokens");

            migrationBuilder.RenameColumn(
                name: "CreatedByUserId",
                table: "People",
                newName: "CreatedByUserId");

            migrationBuilder.RenameIndex(
                name: "IX_People_CreatedByUserId",
                table: "People",
                newName: "IX_People_CreatedByUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_People_Users_CreatedByUserId",
                table: "People",
                column: "CreatedByUserId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
