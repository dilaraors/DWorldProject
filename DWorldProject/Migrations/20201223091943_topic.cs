using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace DWorldProject.Migrations
{
    public partial class topic : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

            migrationBuilder.AddColumn<int>(
                name: "Section",
                table: "BlogPosts",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TopicId",
                table: "BlogPosts",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "VideoUrl",
                table: "BlogPosts",
                type: "text",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Topics",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: true),
                    Section = table.Column<int>(type: "integer", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    DeletedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Topics", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BlogPosts_TopicId",
                table: "BlogPosts",
                column: "TopicId");

            migrationBuilder.AddForeignKey(
                name: "FK_BlogPosts_Topics_TopicId",
                table: "BlogPosts",
                column: "TopicId",
                principalTable: "Topics",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BlogPosts_Topics_TopicId",
                table: "BlogPosts");

            migrationBuilder.DropTable(
                name: "Topics");

            migrationBuilder.DropIndex(
                name: "IX_BlogPosts_TopicId",
                table: "BlogPosts");

            migrationBuilder.DropColumn(
                name: "HeaderImageUrl",
                table: "BlogPosts");

            migrationBuilder.DropColumn(
                name: "ImageUrls",
                table: "BlogPosts");

            migrationBuilder.DropColumn(
                name: "Section",
                table: "BlogPosts");

            migrationBuilder.DropColumn(
                name: "TopicId",
                table: "BlogPosts");

            migrationBuilder.DropColumn(
                name: "VideoUrl",
                table: "BlogPosts");
        }
    }
}
