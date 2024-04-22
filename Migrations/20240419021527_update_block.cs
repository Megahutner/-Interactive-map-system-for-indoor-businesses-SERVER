using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UWEServer.Migrations
{
    /// <inheritdoc />
    public partial class update_block : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ZoneLinkId",
                table: "Blocks",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ZoneLinkId",
                table: "Blocks");
        }
    }
}
