using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Integration.data.Migrations
{
    /// <inheritdoc />
    public partial class init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SecurityStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "bit", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "dataBases",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DbName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ConnectionString = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    dataBaseType = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_dataBases", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "staticStandards",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Control = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    standard = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_staticStandards", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderKey = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "modules",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    priority = table.Column<int>(type: "int", nullable: false),
                    TableFromName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TableToName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ToPrimaryKeyName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    fromPrimaryKeyName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LocalIdName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CloudIdName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ToDbId = table.Column<int>(type: "int", nullable: false),
                    FromDbId = table.Column<int>(type: "int", nullable: false),
                    SyncType = table.Column<int>(type: "int", nullable: false),
                    ToInsertFlagName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ToUpdateFlagName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FromInsertFlagName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FromUpdateFlagName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ToDeleteFlagName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FromDeleteFlagName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    condition = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    isDisabled = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_modules", x => x.Id);
                    table.ForeignKey(
                        name: "FK_modules_dataBases_FromDbId",
                        column: x => x.FromDbId,
                        principalTable: "dataBases",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_modules_dataBases_ToDbId",
                        column: x => x.ToDbId,
                        principalTable: "dataBases",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "columnFroms",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ColumnFromName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ColumnToName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ModuleId = table.Column<int>(type: "int", nullable: false),
                    isReference = table.Column<bool>(type: "bit", nullable: false),
                    isAlter = table.Column<bool>(type: "bit", nullable: false),
                    TableReferenceName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Key = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_columnFroms", x => x.Id);
                    table.ForeignKey(
                        name: "FK_columnFroms_modules_ModuleId",
                        column: x => x.ModuleId,
                        principalTable: "modules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Operations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LocalId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CloudId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TableFrom = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TableTo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PriceFrom = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PriceTo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ItemId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    fromInsertFlag = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    fromUpdateFlag = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    fromDeleteFlag = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ToInsertFlag = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ToUpdateFlag = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ToDeleteFlag = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Condition = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    fromPriceInsertDate = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OpCustomerReference = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OPProductReference = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OPTOSellerPrimary = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OpSellerReference = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    customerId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    storeId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OPToItemPrimary = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    operationType = table.Column<int>(type: "int", nullable: false),
                    OpToCustomerId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OpToProductId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ModuleId = table.Column<int>(type: "int", nullable: false),
                    OpFromPrimary = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FromItemParent = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OpFromInsertDate = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OpFromUpdateDate = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OpFromDeleteDate = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Operations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Operations_modules_ModuleId",
                        column: x => x.ModuleId,
                        principalTable: "modules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "References",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TableFromName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LocalName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PrimaryName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Alter = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ModuleId = table.Column<int>(type: "int", nullable: false),
                    Key = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_References", x => x.Id);
                    table.ForeignKey(
                        name: "FK_References_modules_ModuleId",
                        column: x => x.ModuleId,
                        principalTable: "modules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_columnFroms_ModuleId",
                table: "columnFroms",
                column: "ModuleId");

            migrationBuilder.CreateIndex(
                name: "IX_modules_FromDbId",
                table: "modules",
                column: "FromDbId");

            migrationBuilder.CreateIndex(
                name: "IX_modules_ToDbId",
                table: "modules",
                column: "ToDbId");

            migrationBuilder.CreateIndex(
                name: "IX_Operations_ModuleId",
                table: "Operations",
                column: "ModuleId");

            migrationBuilder.CreateIndex(
                name: "IX_References_ModuleId",
                table: "References",
                column: "ModuleId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "columnFroms");

            migrationBuilder.DropTable(
                name: "Operations");

            migrationBuilder.DropTable(
                name: "References");

            migrationBuilder.DropTable(
                name: "staticStandards");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "modules");

            migrationBuilder.DropTable(
                name: "dataBases");
        }
    }
}
