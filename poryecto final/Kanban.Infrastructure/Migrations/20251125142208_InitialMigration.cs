using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Kanban.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialMigration : Migration
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
                    PasswordHash = table.Column<string>(type: "text", nullable: false),
                    Role = table.Column<string>(type: "text", nullable: false),
                    FullName = table.Column<string>(type: "text", nullable: false)
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
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UserId1 = table.Column<int>(type: "integer", nullable: true)
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
                    table.ForeignKey(
                        name: "FK_Tasks_Users_UserId1",
                        column: x => x.UserId1,
                        principalTable: "Users",
                        principalColumn: "Id");
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
                    { 1, "admin@test.com", "Admin", "$2a$11$DEBJHZfJKIyA5Jcib/VvEexatn8Ye.yCDe5jYdl8uvhoejqUVSFy6", "Admin" },
                    { 2, "user@test.com", "User 1", "$2a$11$AbYmJN3StqXRSMzkuo9kjeZq.tL.hN.ZtuBGIUILMH34zzDGN0/9u", "User" },
                    { 3, "user2@test.com", "User 2", "$2a$11$G3MkrF3XCfUQR6n1FEnkYOHfnRYRFBoww.2Tp1lOz.ggClAC0fbHi", "User" },
                    { 999, "doc_js_galindo@fesc.edu.co", "Docente FESC", "$2a$11$82gOG.bjzg0ovLhfyEXlBuaJHtBNuVXyg1.nO8T3S9PsLNAywA1yS", "Admin" }
                });

            migrationBuilder.InsertData(
                table: "Tasks",
                columns: new[] { "Id", "AssignedToUserId", "Comments", "CreatedAt", "Description", "DueDate", "Position", "Priority", "Status", "Tags", "Title", "UpdatedAt", "UserId", "UserId1", "UserName" },
                values: new object[,]
                {
                    { 1, null, null, new DateTime(2025, 11, 20, 14, 22, 8, 240, DateTimeKind.Utc).AddTicks(8310), "Configurar PostgreSQL y migraciones", null, 0, "High", "Done", null, "Configurar base de datos", new DateTime(2025, 11, 24, 14, 22, 8, 240, DateTimeKind.Utc).AddTicks(8316), 1, null, "" },
                    { 2, null, null, new DateTime(2025, 11, 22, 14, 22, 8, 240, DateTimeKind.Utc).AddTicks(8319), "Sistema de login y JWT", null, 0, "Medium", "InProgress", null, "Implementar autenticación", new DateTime(2025, 11, 25, 14, 22, 8, 240, DateTimeKind.Utc).AddTicks(8319), 2, null, "" },
                    { 3, null, null, new DateTime(2025, 11, 24, 14, 22, 8, 240, DateTimeKind.Utc).AddTicks(8371), "Mockups y diseño UI", new DateTime(2025, 12, 2, 14, 22, 8, 240, DateTimeKind.Utc).AddTicks(8368), 0, "Low", "ToDo", "UI,Design", "Diseñar interfaz", new DateTime(2025, 11, 24, 14, 22, 8, 240, DateTimeKind.Utc).AddTicks(8372), 3, null, "" },
                    { 4, null, null, new DateTime(2025, 11, 25, 14, 22, 8, 240, DateTimeKind.Utc).AddTicks(8374), "Tarea especial del profesor", null, 0, "Medium", "ToDo", null, "Tarea para revisión docente", new DateTime(2025, 11, 25, 14, 22, 8, 240, DateTimeKind.Utc).AddTicks(8374), 999, null, "" }
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

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_UserId1",
                table: "Tasks",
                column: "UserId1");
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
