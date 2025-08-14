using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddQuizContent : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SharedQuizQuestions");

            migrationBuilder.DropColumn(
                name: "Difficulty",
                table: "SharedQuizzes");

            migrationBuilder.DropColumn(
                name: "Topic",
                table: "SharedQuizzes");

            migrationBuilder.AddColumn<Guid>(
                name: "QuizContentId",
                table: "SharedQuizzes",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "QuizContents",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Topic = table.Column<string>(type: "text", nullable: false),
                    Difficulty = table.Column<int>(type: "integer", nullable: false),
                    ContentHash = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuizContents", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "QuizContentQuestions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Question = table.Column<string>(type: "text", nullable: false),
                    Options = table.Column<List<string>>(type: "text[]", nullable: false),
                    CorrectAnswer = table.Column<string>(type: "text", nullable: false),
                    QuizContentId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuizContentQuestions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_QuizContentQuestions_QuizContents_QuizContentId",
                        column: x => x.QuizContentId,
                        principalTable: "QuizContents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SharedQuizzes_QuizContentId",
                table: "SharedQuizzes",
                column: "QuizContentId");

            migrationBuilder.CreateIndex(
                name: "IX_QuizContentQuestions_QuizContentId",
                table: "QuizContentQuestions",
                column: "QuizContentId");

            migrationBuilder.AddForeignKey(
                name: "FK_SharedQuizzes_QuizContents_QuizContentId",
                table: "SharedQuizzes",
                column: "QuizContentId",
                principalTable: "QuizContents",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SharedQuizzes_QuizContents_QuizContentId",
                table: "SharedQuizzes");

            migrationBuilder.DropTable(
                name: "QuizContentQuestions");

            migrationBuilder.DropTable(
                name: "QuizContents");

            migrationBuilder.DropIndex(
                name: "IX_SharedQuizzes_QuizContentId",
                table: "SharedQuizzes");

            migrationBuilder.DropColumn(
                name: "QuizContentId",
                table: "SharedQuizzes");

            migrationBuilder.AddColumn<int>(
                name: "Difficulty",
                table: "SharedQuizzes",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Topic",
                table: "SharedQuizzes",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "SharedQuizQuestions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SharedQuizId = table.Column<Guid>(type: "uuid", nullable: false),
                    CorrectAnswer = table.Column<string>(type: "text", nullable: false),
                    Options = table.Column<List<string>>(type: "text[]", nullable: false),
                    Question = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SharedQuizQuestions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SharedQuizQuestions_SharedQuizzes_SharedQuizId",
                        column: x => x.SharedQuizId,
                        principalTable: "SharedQuizzes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SharedQuizQuestions_SharedQuizId",
                table: "SharedQuizQuestions",
                column: "SharedQuizId");
        }
    }
}
