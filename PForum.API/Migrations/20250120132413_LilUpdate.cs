using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PForum.API.Migrations
{
    /// <inheritdoc />
    public partial class LilUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TopicName",
                table: "TopicThreads",
                newName: "UserId");

            migrationBuilder.RenameColumn(
                name: "QuestionMessageId",
                table: "TopicThreads",
                newName: "ThreadName");

            migrationBuilder.AddColumn<int>(
                name: "Role",
                table: "Users",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "PostedAt",
                table: "TopicThreads",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "ThreadDescription",
                table: "TopicThreads",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TopicThreads_UserId",
                table: "TopicThreads",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_TopicThreads_Users_UserId",
                table: "TopicThreads",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TopicThreads_Users_UserId",
                table: "TopicThreads");

            migrationBuilder.DropIndex(
                name: "IX_Users_Email",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_TopicThreads_UserId",
                table: "TopicThreads");

            migrationBuilder.DropColumn(
                name: "Role",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "PostedAt",
                table: "TopicThreads");

            migrationBuilder.DropColumn(
                name: "ThreadDescription",
                table: "TopicThreads");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "TopicThreads",
                newName: "TopicName");

            migrationBuilder.RenameColumn(
                name: "ThreadName",
                table: "TopicThreads",
                newName: "QuestionMessageId");
        }
    }
}
