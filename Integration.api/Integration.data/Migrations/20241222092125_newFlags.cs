using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Integration.data.Migrations
{
    /// <inheritdoc />
    public partial class newFlags : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ItemParent",
                table: "Operations",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OpDeleteDate",
                table: "Operations",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OpFromPrimary",
                table: "Operations",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OpInsertDate",
                table: "Operations",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OpUpdateDate",
                table: "Operations",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ItemParent",
                table: "Operations");

            migrationBuilder.DropColumn(
                name: "OpDeleteDate",
                table: "Operations");

            migrationBuilder.DropColumn(
                name: "OpFromPrimary",
                table: "Operations");

            migrationBuilder.DropColumn(
                name: "OpInsertDate",
                table: "Operations");

            migrationBuilder.DropColumn(
                name: "OpUpdateDate",
                table: "Operations");
        }
    }
}
