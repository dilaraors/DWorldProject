using Microsoft.EntityFrameworkCore.Migrations;

namespace DWorldProject.Migrations
{
    public partial class youtubeVideoUrl : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "YouTubeVideoURL",
                table: "BlogPosts",
                type: "text",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "YouTubeVideoURL",
                table: "BlogPosts");
        }
    }
}
