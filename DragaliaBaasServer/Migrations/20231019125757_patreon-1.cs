using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DragaliaBaasServer.Migrations
{
    /// <inheritdoc />
    public partial class patreon1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "LinkedPatreonUserId",
                table: "WebUsers",
                type: "character varying(32)",
                maxLength: 32,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LinkedPatreonUserId",
                table: "WebUsers");
        }
    }
}
