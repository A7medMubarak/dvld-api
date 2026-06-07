using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DVLD.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddCreatedByUserIdToPerson : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Role",
                table: "Users",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "CreatedByUserId",
                table: "People",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_People_CreatedByUserId",
                table: "People",
                column: "CreatedByUserId");

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

            migrationBuilder.DropIndex(
                name: "IX_People_CreatedByUserId",
                table: "People");

            migrationBuilder.DropColumn(
                name: "Role",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "People");
        }
    }
}
