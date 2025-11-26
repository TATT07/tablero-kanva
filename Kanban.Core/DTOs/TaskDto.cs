using System;
using Kanban.Core.Entities;

namespace Kanban.Core.DTOs;

public class TaskDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public Entities.TaskStatus Status { get; set; }
    public int Position { get; set; }

    // Usuario propietario
    public int UserId { get; set; }
    public string UserName { get; set; } = string.Empty;

    // Usuario asignado
    public int? AssignedToUserId { get; set; }
    public string? AssignedToUserName { get; set; }

    // Nuevos campos
    public DateTime? DueDate { get; set; }
    public TaskPriority Priority { get; set; }
    public string? Tags { get; set; }
    public string? Comments { get; set; }

    // Timestamps
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}