using System;
using System.Collections.Generic;

namespace Kanban.Core.Entities;

public class Task
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public TaskStatus Status { get; set; } = TaskStatus.ToDo;
    public int Position { get; set; } = 0;

    // Usuario propietario
    public int UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public User User { get; set; } = null!;

    // Usuario asignado (opcional)
    public int? AssignedToUserId { get; set; }
    public User? AssignedToUser { get; set; }

    // Nuevos atributos
    public DateTime? DueDate { get; set; }
    public TaskPriority Priority { get; set; } = TaskPriority.Medium;
    public string? Tags { get; set; } // Separados por coma
    public string? Comments { get; set; } // Campo de texto para notas/comentarios

    // Timestamps
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navegación para historial
    public ICollection<TaskHistory> History { get; set; } = new List<TaskHistory>();
}

public enum TaskPriority
{
    Low = 0,
    Medium = 1,
    High = 2,
    Urgent = 3
}