using System;
using Kanban.Core.Entities;

namespace Kanban.Core.DTOs;

public class TaskFilters
{
    public string? Search { get; set; }
    public Entities.TaskStatus? Status { get; set; }
    public TaskPriority? Priority { get; set; }
    public DateTime? DueDateFrom { get; set; }
    public DateTime? DueDateTo { get; set; }
    public string? SortBy { get; set; } = "CreatedAt";
    public bool SortDescending { get; set; } = true;
}