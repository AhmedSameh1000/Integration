using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Integration.data.Migrations
{
    /// <inheritdoc />
    public partial class Edits : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "OpUpdateDate",
                table: "Operations",
                newName: "OpFromUpdateDate");

            migrationBuilder.RenameColumn(
                name: "OpInsertDate",
                table: "Operations",
                newName: "OpFromInsertDate");

            migrationBuilder.RenameColumn(
                name: "OpDeleteDate",
                table: "Operations",
                newName: "OpFromDeleteDate");

            migrationBuilder.RenameColumn(
                name: "ItemParent",
                table: "Operations",
                newName: "FromItemParent");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "OpFromUpdateDate",
                table: "Operations",
                newName: "OpUpdateDate");

            migrationBuilder.RenameColumn(
                name: "OpFromInsertDate",
                table: "Operations",
                newName: "OpInsertDate");

            migrationBuilder.RenameColumn(
                name: "OpFromDeleteDate",
                table: "Operations",
                newName: "OpDeleteDate");

            migrationBuilder.RenameColumn(
                name: "FromItemParent",
                table: "Operations",
                newName: "ItemParent");
        }
    }
}
