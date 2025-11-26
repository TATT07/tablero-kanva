using System;

namespace Kanban.Core.Entities
{
    public class User
    {
        public int Id { get; set; }

        public string Email { get; set; } = string.Empty;

        public string PasswordHash { get; set; } = string.Empty;

        public string Role { get; set; } = "User";

        // 🔥 ESTE ES EL CAMPO QUE TE FALTA
        public string FullName { get; set; } = string.Empty;

        // Relaciones
        public ICollection<Task>? Tasks { get; set; }
    }
}
