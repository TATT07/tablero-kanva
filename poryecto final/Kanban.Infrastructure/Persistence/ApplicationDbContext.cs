using System;
using Microsoft.EntityFrameworkCore;
using Kanban.Core.Entities;

namespace Kanban.Infrastructure.Persistence
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; } = null!;
        public DbSet<Kanban.Core.Entities.Task> Tasks { get; set; } = null!;
        public DbSet<RefreshToken> RefreshTokens { get; set; } = null!;
        public DbSet<TaskHistory> TaskHistories { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Fix enum conversions
            modelBuilder.Entity<Kanban.Core.Entities.Task>()
                .Property(t => t.Status)
                .HasConversion<string>();

            modelBuilder.Entity<Kanban.Core.Entities.Task>()
                .Property(t => t.Priority)
                .HasConversion<string>();

            // Relationships
            modelBuilder.Entity<Kanban.Core.Entities.Task>()
                .HasOne(t => t.User)
                .WithMany()
                .HasForeignKey(t => t.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Kanban.Core.Entities.Task>()
                .HasOne(t => t.AssignedToUser)
                .WithMany()
                .HasForeignKey(t => t.AssignedToUserId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<TaskHistory>()
                .HasOne(th => th.Task)
                .WithMany(t => t.History)
                .HasForeignKey(th => th.TaskId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<TaskHistory>()
                .HasOne(th => th.User)
                .WithMany()
                .HasForeignKey(th => th.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            SeedData(modelBuilder);
        }

        private void SeedData(ModelBuilder modelBuilder)
        {
            // ==== USERS (✔ con FullName) ====
            modelBuilder.Entity<User>().HasData(
                new User
                {
                    Id = 1,
                    Email = "admin@test.com",
                    FullName = "Admin",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("0123456789"),
                    Role = "Admin"
                },
                new User
                {
                    Id = 2,
                    Email = "user@test.com",
                    FullName = "User 1",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("user123"),
                    Role = "User"
                },
                new User
                {
                    Id = 3,
                    Email = "user2@test.com",
                    FullName = "User 2",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("user123"),
                    Role = "User"
                },
                new User
                {
                    Id = 999,
                    Email = "doc_js_galindo@fesc.edu.co",
                    FullName = "Docente FESC",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("0123456789"),
                    Role = "Admin"
                }
            );

            // ==== TASKS ====
            modelBuilder.Entity<Kanban.Core.Entities.Task>().HasData(
                new Kanban.Core.Entities.Task
                {
                    Id = 1,
                    Title = "Configurar base de datos",
                    Description = "Configurar PostgreSQL y migraciones",
                    Status = Kanban.Core.Entities.TaskStatus.Done,
                    Priority = TaskPriority.High,
                    UserId = 1,
                    Position = 0,
                    CreatedAt = DateTime.UtcNow.AddDays(-5),
                    UpdatedAt = DateTime.UtcNow.AddDays(-1)
                },
                new Kanban.Core.Entities.Task
                {
                    Id = 2,
                    Title = "Implementar autenticación",
                    Description = "Sistema de login y JWT",
                    Status = Kanban.Core.Entities.TaskStatus.InProgress,
                    Priority = TaskPriority.Medium,
                    UserId = 2,
                    Position = 0,
                    CreatedAt = DateTime.UtcNow.AddDays(-3),
                    UpdatedAt = DateTime.UtcNow
                },
                new Kanban.Core.Entities.Task
                {
                    Id = 3,
                    Title = "Diseñar interfaz",
                    Description = "Mockups y diseño UI",
                    Status = Kanban.Core.Entities.TaskStatus.ToDo,
                    Priority = TaskPriority.Low,
                    UserId = 3,
                    Position = 0,
                    DueDate = DateTime.UtcNow.AddDays(7),
                    Tags = "UI,Design",
                    CreatedAt = DateTime.UtcNow.AddDays(-1),
                    UpdatedAt = DateTime.UtcNow.AddDays(-1)
                },
                new Kanban.Core.Entities.Task
                {
                    Id = 4,
                    Title = "Tarea para revisión docente",
                    Description = "Tarea especial del profesor",
                    Status = Kanban.Core.Entities.TaskStatus.ToDo,
                    Priority = TaskPriority.Medium,
                    UserId = 999,
                    Position = 0,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                }
            );
        }
    }
}
