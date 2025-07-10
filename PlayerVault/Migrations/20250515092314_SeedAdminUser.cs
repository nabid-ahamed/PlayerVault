using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PlayerVault.Migrations
{
    /// <inheritdoc />
    public partial class SeedAdminUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "Email", "IsVerified", "PasswordHash", "Role", "Username" },
                values: new object[] { 1, "admin@playervault.com", true, "$2a$11$hNvaWJrrt6PXiWGbmvQB2OPMMqoGhagn77RJ1xhdyBa/OSctHgKzW", "Admin", "admin" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1);
        }
    }
}
