using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UWEServer.Migrations
{
    /// <inheritdoc />
    public partial class add_category : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CategoryId",
                table: "Blocks",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Category",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DelFlag = table.Column<bool>(type: "bit", nullable: false),
                    CreatedTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Category", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Blocks_CategoryId",
                table: "Blocks",
                column: "CategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_Blocks_Category_CategoryId",
                table: "Blocks",
                column: "CategoryId",
                principalTable: "Category",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Blocks_Category_CategoryId",
                table: "Blocks");

            migrationBuilder.DropTable(
                name: "Category");

            migrationBuilder.DropIndex(
                name: "IX_Blocks_CategoryId",
                table: "Blocks");

            migrationBuilder.DropColumn(
                name: "CategoryId",
                table: "Blocks");
        }
    }
}
