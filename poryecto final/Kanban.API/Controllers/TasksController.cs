using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Kanban.Application.Services;
using Kanban.Core.DTOs;
using Kanban.Core.Entities;
using Kanban.Infrastructure.Persistence;

namespace Kanban.API.Controllers;

[ApiController]
[Route("api/tasks")]
// [Authorize]
public class TasksController : ControllerBase
{
    private readonly TaskService _taskService;
    private readonly ApplicationDbContext _context;

    public TasksController(TaskService taskService, ApplicationDbContext context)
    {
        _taskService = taskService;
        _context = context;
    }

    private int GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userIdClaim == null)
        {
            Console.WriteLine("[TASKS] ERROR: Missing user claim (NameIdentifier)");
            return 0;
        }

        if (!int.TryParse(userIdClaim, out var userId))
        {
            Console.WriteLine($"[TASKS] ERROR: Invalid userId claim: {userIdClaim}");
            return 0;
        }

        Console.WriteLine($"[TASKS] UserId: {userId}");
        return userId;
    }

    private string GetCurrentUserRole()
    {
        var roleClaim = User.FindFirst(ClaimTypes.Role)?.Value ?? User.FindFirst("role")?.Value ?? "User";
        Console.WriteLine($"[TASKS] UserRole: {roleClaim}");
        return roleClaim;
    }

    [HttpGet("ping")]
    public IActionResult Ping()
    {
        return Ok(new { message = "tasks ping" });
    }

    [HttpGet("debug")]
    public async System.Threading.Tasks.Task<IActionResult> Debug()
    {
        var userId = GetCurrentUserId();
        var userRole = GetCurrentUserRole();

        var usersCount = await _context.Users.CountAsync();
        var tasksCount = await _context.Tasks.CountAsync();

        return Ok(new {
            userId,
            userRole,
            usersCount,
            tasksCount,
            users = await _context.Users.Select(u => new { u.Id, u.Email, u.Role }).ToListAsync()
        });
    }

    [HttpGet]
    public async System.Threading.Tasks.Task<IActionResult> GetTasks(
        [FromQuery] string? search = null,
        [FromQuery] Kanban.Core.Entities.TaskStatus? status = null,
        [FromQuery] TaskPriority? priority = null,
        [FromQuery] DateTime? dueDateFrom = null,
        [FromQuery] DateTime? dueDateTo = null,
        [FromQuery] string? sortBy = "CreatedAt",
        [FromQuery] bool sortDescending = true)
    {
        var userId = GetCurrentUserId();
        var userRole = GetCurrentUserRole();

        var tasks = await _taskService.GetTasksAsync(userId, userRole == "Admin", new TaskFilters
        {
            Search = search,
            Status = status,
            Priority = priority,
            DueDateFrom = dueDateFrom,
            DueDateTo = dueDateTo,
            SortBy = sortBy,
            SortDescending = sortDescending
        });

        return Ok(tasks);
    }

    [HttpGet("{id}")]
    public async System.Threading.Tasks.Task<IActionResult> GetTask(int id)
    {
        var userId = GetCurrentUserId();
        var userRole = GetCurrentUserRole();

        var task = await _taskService.GetTaskByIdAsync(id, userId, userRole == "Admin");
        if (task == null) return NotFound();

        return Ok(task);
    }

    [HttpPost]
    public async System.Threading.Tasks.Task<IActionResult> CreateTask(CreateTaskRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var userId = GetCurrentUserId();
        var userRole = GetCurrentUserRole();

        try
        {
            var task = await _taskService.CreateTaskAsync(request, userId);
            return CreatedAtAction(nameof(GetTask), new { id = task.Id }, task);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message, userId, userRole, request });
        }
    }

    [HttpPut("{id}")]
    public async System.Threading.Tasks.Task<IActionResult> UpdateTask(int id, UpdateTaskRequest request)
    {
        var userId = GetCurrentUserId();
        var userRole = GetCurrentUserRole();

        var task = await _taskService.UpdateTaskAsync(id, request, userId, userRole == "Admin");
        if (task == null) return NotFound();
        return Ok(task);
    }

    [HttpDelete("{id}")]
    public async System.Threading.Tasks.Task<IActionResult> DeleteTask(int id)
    {
        var userId = GetCurrentUserId();
        var userRole = GetCurrentUserRole();

        var result = await _taskService.DeleteTaskAsync(id, userId, userRole == "Admin");
        if (!result) return NotFound();
        return NoContent();
    }

    [HttpPut("{id}/move")]
    public async System.Threading.Tasks.Task<IActionResult> MoveTask(int id, MoveTaskRequest request)
    {
        var userId = GetCurrentUserId();
        var userRole = GetCurrentUserRole();

        var task = await _taskService.MoveTaskAsync(id, request, userId, userRole == "Admin");
        if (task == null) return NotFound();
        return Ok(task);
    }

    [HttpGet("{id}/history")]
    public async System.Threading.Tasks.Task<IActionResult> GetTaskHistory(int id)
    {
        var userId = GetCurrentUserId();
        var userRole = GetCurrentUserRole();

        var history = await _taskService.GetTaskHistoryAsync(id, userId, userRole == "Admin");
        return Ok(history);
    }

    [HttpGet("export/pdf")]
    public async System.Threading.Tasks.Task<IActionResult> ExportTasksToPdf()
    {
        var userId = GetCurrentUserId();
        var userRole = GetCurrentUserRole();

        var pdfData = await _taskService.ExportTasksToPdfAsync(userId, userRole == "Admin");
        return File(pdfData, "application/pdf", $"kanban-tasks-{DateTime.Now:yyyy-MM-dd}.pdf");
    }

    [HttpGet("stats")]
    public async System.Threading.Tasks.Task<IActionResult> GetTaskStats()
    {
        var userId = GetCurrentUserId();
        var userRole = GetCurrentUserRole();

        var tasks = await _taskService.GetTasksAsync(userId, userRole == "Admin");

        var stats = new
        {
            total = tasks.Count,
            todo = tasks.Count(t => t.Status == Kanban.Core.Entities.TaskStatus.ToDo),
            inProgress = tasks.Count(t => t.Status == Kanban.Core.Entities.TaskStatus.InProgress),
            completed = tasks.Count(t => t.Status == Kanban.Core.Entities.TaskStatus.Done)
        };

        return Ok(stats);
    }

    [HttpGet("export/excel")]
    public async System.Threading.Tasks.Task<IActionResult> ExportTasksToExcel()
    {
        var userId = GetCurrentUserId();
        var userRole = GetCurrentUserRole();

        var excelData = await _taskService.ExportTasksToExcelAsync(userId, userRole == "Admin");
        return File(excelData, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"kanban-tasks-{DateTime.Now:yyyy-MM-dd}.xlsx");
    }

    // Mantener endpoint existente por compatibilidad
    [HttpPost("export")]
    public async System.Threading.Tasks.Task<IActionResult> ExportTasks()
    {
        var userId = GetCurrentUserId();
        var userRole = GetCurrentUserRole();

        var excelData = await _taskService.ExportTasksAsync(userId, userRole == "Admin");
        return File(excelData, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"kanban-tasks-{DateTime.Now:yyyy-MM-dd}.xlsx");
    }
}
