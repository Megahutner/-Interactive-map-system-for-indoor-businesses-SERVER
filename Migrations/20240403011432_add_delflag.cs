using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UWEServer.Migrations
{
    /// <inheritdoc />
    public partial class add_delflag : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "DelFlag",
                table: "Zones",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "DelFlag",
                table: "Terminals",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "DelFlag",
                table: "Blocks",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DelFlag",
                table: "Zones");

            migrationBuilder.DropColumn(
                name: "DelFlag",
                table: "Terminals");

            migrationBuilder.DropColumn(
                name: "DelFlag",
                table: "Blocks");
        }
    }
}
