using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DragaliaBaasServer.Migrations
{
    /// <inheritdoc />
    public partial class ProdAddAutoInclude : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "SavefileUploadedAt",
                table: "WebUsers",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SavefileUploadedAt",
                table: "WebUsers");
        }
    }
}
