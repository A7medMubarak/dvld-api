using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DVLD.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class UpdateRefreshTokenColumnLengths : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Token",
                table: "RefreshTokens",
                type: "nvarchar(44)",
                maxLength: 44,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255);

            migrationBuilder.AlterColumn<string>(
                name: "ReplacedByToken",
                table: "RefreshTokens",
                type: "nvarchar(44)",
                maxLength: 44,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255,
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Token",
                table: "RefreshTokens",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(44)",
                oldMaxLength: 44);

            migrationBuilder.AlterColumn<string>(
                name: "ReplacedByToken",
                table: "RefreshTokens",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(44)",
                oldMaxLength: 44,
                oldNullable: true);
        }
    }
}
