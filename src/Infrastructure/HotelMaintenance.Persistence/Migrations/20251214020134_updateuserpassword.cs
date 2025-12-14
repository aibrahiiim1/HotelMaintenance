using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HotelMaintenance.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class updateuserpassword : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PasswordHash",
                schema: "dbo",
                table: "Users",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PasswordHash",
                schema: "dbo",
                table: "Users");
        }
    }
}
