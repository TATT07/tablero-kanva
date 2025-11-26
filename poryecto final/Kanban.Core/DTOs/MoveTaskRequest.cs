using Kanban.Core.Entities;

namespace Kanban.Core.DTOs;

public class MoveTaskRequest
{
    public Kanban.Core.Entities.TaskStatus Status { get; set; }
    public int Position { get; set; }
}