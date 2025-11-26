using System;

namespace Kanban.Core.Entities;

public class TaskHistory
{
    public int Id { get; set; }
    public int TaskId { get; set; }
    public Task Task { get; set; } = null!;

    public int UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public User User { get; set; } = null!;

    public string Action { get; set; } = string.Empty; // "Created", "Updated", "StatusChanged", "Deleted"
    public string? OldValue { get; set; }
    public string? NewValue { get; set; }
    public string? Details { get; set; } // Descripción detallada del cambio

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}