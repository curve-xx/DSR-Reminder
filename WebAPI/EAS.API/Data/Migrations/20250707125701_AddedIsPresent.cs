using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EAS.API.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddedIsPresent : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsPresent",
                table: "Attendances",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsPresent",
                table: "Attendances");
        }
    }
}
