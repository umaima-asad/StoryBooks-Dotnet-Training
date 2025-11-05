using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StoryBooks.Migrations
{
    /// <inheritdoc />
    public partial class tenants : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TenantID",
                table: "StoryBooks",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TenantID",
                table: "AspNetUsers",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TenantID",
                table: "StoryBooks");

            migrationBuilder.DropColumn(
                name: "TenantID",
                table: "AspNetUsers");
        }
    }
}
