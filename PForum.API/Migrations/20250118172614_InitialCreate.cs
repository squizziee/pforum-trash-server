using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PForum.API.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "LanguageTopics",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    LanguageName = table.Column<string>(type: "TEXT", nullable: false),
                    LanguageDescription = table.Column<string>(type: "TEXT", nullable: false),
                    LanguageLogoUrl = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LanguageTopics", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Email = table.Column<string>(type: "TEXT", nullable: false),
                    Username = table.Column<string>(type: "TEXT", nullable: false),
                    Password = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Topics",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    TopicName = table.Column<string>(type: "TEXT", nullable: false),
                    TopicDescription = table.Column<string>(type: "TEXT", nullable: false),
                    LanguageTopicId = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Topics", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Topics_LanguageTopics_LanguageTopicId",
                        column: x => x.LanguageTopicId,
                        principalTable: "LanguageTopics",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TopicThreads",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    TopicName = table.Column<string>(type: "TEXT", nullable: false),
                    TopicId = table.Column<Guid>(type: "TEXT", nullable: false),
                    QuestionMessageId = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TopicThreads", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TopicThreads_Topics_TopicId",
                        column: x => x.TopicId,
                        principalTable: "Topics",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TopicThreadMessages",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    MessageText = table.Column<string>(type: "TEXT", nullable: false),
                    TopicThreadId = table.Column<Guid>(type: "TEXT", nullable: false),
                    UserId = table.Column<Guid>(type: "TEXT", nullable: false),
                    PostedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TopicThreadMessages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TopicThreadMessages_TopicThreads_TopicThreadId",
                        column: x => x.TopicThreadId,
                        principalTable: "TopicThreads",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TopicThreadMessages_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Topics_LanguageTopicId",
                table: "Topics",
                column: "LanguageTopicId");

            migrationBuilder.CreateIndex(
                name: "IX_TopicThreadMessages_TopicThreadId",
                table: "TopicThreadMessages",
                column: "TopicThreadId");

            migrationBuilder.CreateIndex(
                name: "IX_TopicThreadMessages_UserId",
                table: "TopicThreadMessages",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_TopicThreads_TopicId",
                table: "TopicThreads",
                column: "TopicId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TopicThreadMessages");

            migrationBuilder.DropTable(
                name: "TopicThreads");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Topics");

            migrationBuilder.DropTable(
                name: "LanguageTopics");
        }
    }
}
