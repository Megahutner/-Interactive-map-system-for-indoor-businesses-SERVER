using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UWEServer.Migrations
{
    /// <inheritdoc />
    public partial class add_id_block_terminal : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TerminalId",
                table: "Terminals",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BlockId",
                table: "Blocks",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TerminalId",
                table: "Terminals");

            migrationBuilder.DropColumn(
                name: "BlockId",
                table: "Blocks");
        }
    }
}
