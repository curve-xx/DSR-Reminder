using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EAS.API.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddedIPAddress : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "IPAddress",
                table: "Attendances",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IPAddress",
                table: "Attendances");
        }
    }
}
