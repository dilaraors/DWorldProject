using Microsoft.EntityFrameworkCore.Migrations;

namespace DWorldProject.Migrations
{
    public partial class blogpostsection : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Section",
                table: "BlogPosts");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Section",
                table: "BlogPosts",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }
    }
}
