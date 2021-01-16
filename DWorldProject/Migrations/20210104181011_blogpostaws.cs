using Microsoft.EntityFrameworkCore.Migrations;

namespace DWorldProject.Migrations
{
    public partial class blogpostaws : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AWSImageId",
                table: "BlogPosts",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AWSImageId",
                table: "BlogPosts");
        }
    }
}
