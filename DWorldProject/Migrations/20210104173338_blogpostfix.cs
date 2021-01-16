using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DWorldProject.Migrations
{
    public partial class blogpostfix : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HeaderImageUrl",
                table: "BlogPosts");

            migrationBuilder.DropColumn(
                name: "ImageUrls",
                table: "BlogPosts");

            migrationBuilder.DropColumn(
                name: "VideoUrl",
                table: "BlogPosts");

            migrationBuilder.AddColumn<int>(
                name: "SectionId",
                table: "BlogPosts",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SectionId",
                table: "BlogPosts");

            migrationBuilder.AddColumn<string>(
                name: "HeaderImageUrl",
                table: "BlogPosts",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string[]>(
                name: "ImageUrls",
                table: "BlogPosts",
                type: "text[]",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "VideoUrl",
                table: "BlogPosts",
                type: "text",
                nullable: true);
        }
    }
}
