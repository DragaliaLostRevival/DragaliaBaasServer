using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace DragaliaBaasServer.Migrations
{
    /// <inheritdoc />
    public partial class InitialProd : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "WebUsers",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    Username = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Password = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WebUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    PermissionsPersonalAnalytics = table.Column<bool>(name: "Permissions_PersonalAnalytics", type: "boolean", nullable: false),
                    PermissionsPersonalNotification = table.Column<bool>(name: "Permissions_PersonalNotification", type: "boolean", nullable: false),
                    PermissionsPersonalAnalyticsUpdatedAt = table.Column<decimal>(name: "Permissions_PersonalAnalyticsUpdatedAt", type: "numeric(20,0)", nullable: false),
                    PermissionsPersonalNotificationUpdatedAt = table.Column<decimal>(name: "Permissions_PersonalNotificationUpdatedAt", type: "numeric(20,0)", nullable: false),
                    HasUnreadCsComment = table.Column<bool>(type: "boolean", nullable: false),
                    WebUserAccountId = table.Column<string>(type: "text", nullable: true),
                    ExtendedUserInfoStatus = table.Column<int>(name: "ExtendedUserInfo_Status", type: "integer", nullable: false),
                    ExtendedUserInfoHasUploadedSaveData = table.Column<bool>(name: "ExtendedUserInfo_HasUploadedSaveData", type: "boolean", nullable: false),
                    ExtendedUserInfoBanReason = table.Column<string>(name: "ExtendedUserInfo_BanReason", type: "text", nullable: true),
                    ExtendedUserInfoBanExpiration = table.Column<DateTimeOffset>(name: "ExtendedUserInfo_BanExpiration", type: "timestamp with time zone", nullable: true),
                    ExtendedUserInfoSaveDataDownloadUrl = table.Column<string>(name: "ExtendedUserInfo_SaveDataDownloadUrl", type: "text", nullable: true),
                    CreatedAt = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    UpdatedAt = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    Nickname = table.Column<string>(type: "text", nullable: false),
                    Country = table.Column<string>(type: "text", nullable: false),
                    Birthday = table.Column<string>(type: "text", nullable: false),
                    Gender = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Users_WebUsers_WebUserAccountId",
                        column: x => x.WebUserAccountId,
                        principalTable: "WebUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Devices",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    Password = table.Column<string>(type: "text", nullable: false),
                    UserAccountId = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Devices", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Devices_Users_UserAccountId",
                        column: x => x.UserAccountId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "UserVcmInfo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    VirtualCurrencyName = table.Column<int>(type: "integer", nullable: false),
                    Market = table.Column<int>(type: "integer", nullable: false),
                    UserAccountId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserVcmInfo", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserVcmInfo_Users_UserAccountId",
                        column: x => x.UserAccountId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserVcmBalance",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Free = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    Total = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    Remitted = table.Column<bool>(type: "boolean", nullable: false),
                    UserVcmInfoId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserVcmBalance", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserVcmBalance_UserVcmInfo_UserVcmInfoId",
                        column: x => x.UserVcmInfoId,
                        principalTable: "UserVcmInfo",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "VcmBalancePaidEntry",
                columns: table => new
                {
                    UserVcmBalanceId = table.Column<int>(type: "integer", nullable: false),
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Code = table.Column<string>(type: "text", nullable: false),
                    Total = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VcmBalancePaidEntry", x => new { x.UserVcmBalanceId, x.Id });
                    table.ForeignKey(
                        name: "FK_VcmBalancePaidEntry_UserVcmBalance_UserVcmBalanceId",
                        column: x => x.UserVcmBalanceId,
                        principalTable: "UserVcmBalance",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Devices_UserAccountId",
                table: "Devices",
                column: "UserAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_WebUserAccountId",
                table: "Users",
                column: "WebUserAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_UserVcmBalance_UserVcmInfoId",
                table: "UserVcmBalance",
                column: "UserVcmInfoId");

            migrationBuilder.CreateIndex(
                name: "IX_UserVcmInfo_UserAccountId",
                table: "UserVcmInfo",
                column: "UserAccountId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Devices");

            migrationBuilder.DropTable(
                name: "VcmBalancePaidEntry");

            migrationBuilder.DropTable(
                name: "UserVcmBalance");

            migrationBuilder.DropTable(
                name: "UserVcmInfo");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "WebUsers");
        }
    }
}
