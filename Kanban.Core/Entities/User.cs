namespace Kanban.Core.Entities
{
    public class User
    {
        public int Id { get; set; }

        public string Email { get; set; } = default!;

        public string? FullName { get; set; }

        // Idealmente ya tienes esto
        public string PasswordHash { get; set; } = default!;

        // NUEVO: rol del usuario
        public string Role { get; set; } = "User"; // "Admin" | "User"
    }
}
