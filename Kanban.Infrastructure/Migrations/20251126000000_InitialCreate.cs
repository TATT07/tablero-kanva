using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Kanban.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Email = table.Column<string>(type: "text", nullable: false),
                    FullName = table.Column<string>(type: "text", nullable: true),
                    PasswordHash = table.Column<string>(type: "text", nullable: false),
                    Role = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RefreshTokens",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Token = table.Column<string>(type: "text", nullable: false),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsRevoked = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RefreshTokens", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RefreshTokens_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Tasks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Title = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    Status = table.Column<string>(type: "text", nullable: false),
                    Position = table.Column<int>(type: "integer", nullable: false),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    UserName = table.Column<string>(type: "text", nullable: false),
                    AssignedToUserId = table.Column<int>(type: "integer", nullable: true),
                    DueDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Priority = table.Column<string>(type: "text", nullable: false),
                    Tags = table.Column<string>(type: "text", nullable: true),
                    Comments = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tasks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Tasks_Users_AssignedToUserId",
                        column: x => x.AssignedToUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Tasks_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TaskHistories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TaskId = table.Column<int>(type: "integer", nullable: false),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    UserName = table.Column<string>(type: "text", nullable: false),
                    Action = table.Column<string>(type: "text", nullable: false),
                    OldValue = table.Column<string>(type: "text", nullable: true),
                    NewValue = table.Column<string>(type: "text", nullable: true),
                    Details = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaskHistories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TaskHistories_Tasks_TaskId",
                        column: x => x.TaskId,
                        principalTable: "Tasks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TaskHistories_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "Email", "FullName", "PasswordHash", "Role" },
                values: new object[,]
                {
                    { 1, "doc_js_galindo@fesc.edu.co", "Admin User", "$2a$11$examplehash", "Admin" },
                    { 2, "user@test.com", "Test User", "$2a$11$examplehash", "User" },
                    { 3, "user2@test.com", "Test User 2", "$2a$11$examplehash", "User" }
                });

            migrationBuilder.InsertData(
                table: "Tasks",
                columns: new[] { "Id", "AssignedToUserId", "Comments", "CreatedAt", "Description", "DueDate", "Position", "Priority", "Status", "Tags", "Title", "UpdatedAt", "UserId", "UserName" },
                values: new object[,]
                {
                    { 1, null, null, new DateTime(2025, 11, 21, 5, 0, 0, 0, DateTimeKind.Utc), "Configurar SQL Server y crear migraciones", null, 0, "High", "Done", null, "Configurar base de datos", new DateTime(2025, 11, 25, 5, 0, 0, 0, DateTimeKind.Utc), 1, "doc_js_galindo@fesc.edu.co" },
                    { 2, null, null, new DateTime(2025, 11, 23, 5, 0, 0, 0, DateTimeKind.Utc), "Sistema de login y registro de usuarios", null, 0, "Medium", "InProgress", null, "Implementar autenticación", new DateTime(2025, 11, 26, 5, 57, 6, 825, DateTimeKind.Utc), 2, "user@test.com" },
                    { 3, null, null, new DateTime(2025, 11, 25, 5, 0, 0, 0, DateTimeKind.Utc), "Crear mockups y prototipos de UI", new DateTime(2025, 12, 2, 5, 0, 0, 0, DateTimeKind.Utc), 0, "Low", "ToDo", "UI,Design", "Diseñar interfaz", new DateTime(2025, 11, 25, 5, 0, 0, 0, DateTimeKind.Utc), 3, "user2@test.com" },
                    { 4, null, null, new DateTime(2025, 11, 26, 5, 57, 6, 825, DateTimeKind.Utc), "Esta es una tarea de ejemplo para el usuario docente", null, 0, "Medium", "ToDo", null, "Tarea de ejemplo para docente", new DateTime(2025, 11, 26, 5, 57, 6, 825, DateTimeKind.Utc), 999, "user999@example.com" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_RefreshTokens_UserId",
                table: "RefreshTokens",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_TaskHistories_TaskId",
                table: "TaskHistories",
                column: "TaskId");

            migrationBuilder.CreateIndex(
                name: "IX_TaskHistories_UserId",
                table: "TaskHistories",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_AssignedToUserId",
                table: "Tasks",
                column: "AssignedToUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_UserId",
                table: "Tasks",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RefreshTokens");

            migrationBuilder.DropTable(
                name: "TaskHistories");

            migrationBuilder.DropTable(
                name: "Tasks");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}