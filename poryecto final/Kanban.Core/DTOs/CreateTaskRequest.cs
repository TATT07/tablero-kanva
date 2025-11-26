using System;
using System.ComponentModel.DataAnnotations;
using Kanban.Core.Entities;

namespace Kanban.Core.DTOs;

public class CreateTaskRequest
{
    [Required(ErrorMessage = "Title is required")]
    [StringLength(100, MinimumLength = 1, ErrorMessage = "Title must be between 1 and 100 characters")]
    public string Title { get; set; } = string.Empty;

    [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
    public string? Description { get; set; }

    public Entities.TaskStatus Status { get; set; } = Entities.TaskStatus.ToDo;
    public int Position { get; set; } = 0;

    // Usuario asignado (opcional)
    public int? AssignedToUserId { get; set; }

    // Nuevos campos
    public DateTime? DueDate { get; set; }
    public TaskPriority Priority { get; set; } = TaskPriority.Medium;
    public string? Tags { get; set; }
    public string? Comments { get; set; }
}