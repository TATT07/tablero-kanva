using System.Linq;
using Microsoft.EntityFrameworkCore;
using Kanban.Core.DTOs;
using Kanban.Core.Entities;
using Kanban.Infrastructure.Persistence;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using ClosedXML.Excel;
using Kanban.Core.Entities;

namespace Kanban.Application.Services;

public class TaskService
{
    private readonly ApplicationDbContext _context;

    public TaskService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<TaskDto>> GetTasksAsync(int userId, bool isAdmin, TaskFilters? filters = null)
    {
        var query = _context.Tasks
            .Include(t => t.User)
            .Include(t => t.AssignedToUser)
            .AsQueryable();

        // Filtrar por usuario (solo propias a menos que sea admin)
        if (!isAdmin)
        {
            query = query.Where(t => t.UserId == userId);
        }

        // Aplicar filtros
        if (filters != null)
        {
            if (!string.IsNullOrEmpty(filters.Search))
            {
                var search = filters.Search.ToLower();
                query = query.Where(t =>
                    t.Title.ToLower().Contains(search) ||
                    (t.Description != null && t.Description.ToLower().Contains(search)) ||
                    (t.Tags != null && t.Tags.ToLower().Contains(search)));
            }

            if (filters.Status.HasValue)
            {
                query = query.Where(t => t.Status == filters.Status.Value);
            }

            if (filters.Priority.HasValue)
            {
                query = query.Where(t => t.Priority == filters.Priority.Value);
            }

            if (filters.DueDateFrom.HasValue)
            {
                query = query.Where(t => t.DueDate >= filters.DueDateFrom.Value);
            }

            if (filters.DueDateTo.HasValue)
            {
                query = query.Where(t => t.DueDate <= filters.DueDateTo.Value);
            }

            // Ordenamiento
            var sortProperty = filters.SortBy switch
            {
                "Title" => "Title",
                "Status" => "Status",
                "Priority" => "Priority",
                "DueDate" => "DueDate",
                "CreatedAt" => "CreatedAt",
                _ => "CreatedAt"
            };

            query = filters.SortDescending
                ? query.OrderByDescending(t => EF.Property<object>(t, sortProperty))
                : query.OrderBy(t => EF.Property<object>(t, sortProperty));
        }
        else
        {
            // Ordenamiento por defecto
            query = query.OrderBy(t => t.Status).ThenBy(t => t.Position);
        }

        var tasks = await query.ToListAsync();

        return tasks.Select(MapToDto).ToList();
    }

    public async Task<TaskDto?> GetTaskByIdAsync(int id, int userId, bool isAdmin)
    {
        var query = _context.Tasks
            .Include(t => t.User)
            .Include(t => t.AssignedToUser)
            .AsQueryable();

        if (!isAdmin)
        {
            query = query.Where(t => t.UserId == userId);
        }

        var task = await query.FirstOrDefaultAsync(t => t.Id == id);
        return task != null ? MapToDto(task) : null;
    }

    public async Task<TaskDto> CreateTaskAsync(CreateTaskRequest request, int userId)
    {
        var maxPosition = await _context.Tasks
            .Where(t => t.UserId == userId && t.Status == request.Status)
            .MaxAsync(t => (int?)t.Position) ?? 0;

        // Get the user to set UserName
        var user = await _context.Users.FindAsync(userId);
        if (user == null)
        {
            // Create a default user for bypass
            user = new Kanban.Core.Entities.User
            {
                Id = userId,
                Email = $"user{userId}@example.com",
                PasswordHash = "bypass",
                Role = "Admin"
            };
            _context.Users.Add(user);
            await _context.SaveChangesAsync(); // Commit user first
        }
        var userName = user.Email;

        var task = new Kanban.Core.Entities.Task
        {
            Title = request.Title,
            Description = request.Description,
            Status = request.Status,
            Position = maxPosition + 1,
            UserId = userId,
            UserName = userName, // Set UserName from user or default
            AssignedToUserId = request.AssignedToUserId,
            DueDate = request.DueDate,
            Priority = request.Priority,
            Tags = request.Tags,
            Comments = request.Comments
        };

        _context.Tasks.Add(task);
        await _context.SaveChangesAsync();

        // Registrar en historial (no fallar si hay error)
        try
        {
            await AddHistoryEntryAsync(task.Id, userId, "Created", null, $"Task created with title: {task.Title}");
        }
        catch (Exception ex)
        {
            // Log the error but don't fail the task creation
            Console.WriteLine($"Error logging task history: {ex.Message}");
        }

        return MapToDto(task);
    }

    public async Task<TaskDto?> UpdateTaskAsync(int id, UpdateTaskRequest request, int userId, bool isAdmin)
    {
        var query = _context.Tasks.AsQueryable();
        if (!isAdmin)
        {
            query = query.Where(t => t.UserId == userId);
        }

        var task = await query.FirstOrDefaultAsync(t => t.Id == id);
        if (task == null) return null;

        // Guardar valores anteriores para historial
        var oldValues = new
        {
            task.Title,
            task.Description,
            task.Status,
            task.AssignedToUserId,
            task.DueDate,
            task.Priority,
            task.Tags,
            task.Comments
        };

        // Actualizar campos
        task.Title = request.Title;
        task.Description = request.Description;
        task.Status = request.Status;
        task.Position = request.Position;
        task.AssignedToUserId = request.AssignedToUserId;
        task.DueDate = request.DueDate;
        task.Priority = request.Priority;
        task.Tags = request.Tags;
        task.Comments = request.Comments;
        task.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        // Registrar cambios en historial
        await LogTaskChangesAsync(task, oldValues, userId);

        return MapToDto(task);
    }

    public async Task<bool> DeleteTaskAsync(int id, int userId, bool isAdmin)
    {
        var query = _context.Tasks.AsQueryable();
        if (!isAdmin)
        {
            query = query.Where(t => t.UserId == userId);
        }

        var task = await query.FirstOrDefaultAsync(t => t.Id == id);
        if (task == null) return false;

        _context.Tasks.Remove(task);
        await _context.SaveChangesAsync();

        // Registrar en historial
        await AddHistoryEntryAsync(id, userId, "Deleted", $"Task '{task.Title}' was deleted", null);

        return true;
    }

    public async Task<TaskDto?> MoveTaskAsync(int id, MoveTaskRequest request, int userId, bool isAdmin)
    {
        var query = _context.Tasks.AsQueryable();
        if (!isAdmin)
        {
            query = query.Where(t => t.UserId == userId);
        }

        var task = await query.FirstOrDefaultAsync(t => t.Id == id);
        if (task == null) return null;

        var oldStatus = task.Status;
        var oldPosition = task.Position;

        // Ajustar posiciones
        await AdjustPositionsAsync(task, request.Status, request.Position, userId);

        task.Status = request.Status;
        task.Position = request.Position;
        task.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        // Registrar cambio de estado si aplica
        if (oldStatus != request.Status)
        {
            await AddHistoryEntryAsync(task.Id, userId, "StatusChanged",
                $"{oldStatus} → {request.Status}",
                $"Status changed from {oldStatus} to {request.Status}");
        }

        return MapToDto(task);
    }

    public async Task<List<TaskHistoryDto>> GetTaskHistoryAsync(int taskId, int userId, bool isAdmin)
    {
        var query = _context.TaskHistories
            .Include(h => h.User)
            .Where(h => h.TaskId == taskId)
            .AsQueryable();

        if (!isAdmin)
        {
            // Verificar que el usuario tenga acceso a la tarea
            var task = await _context.Tasks.FirstOrDefaultAsync(t => t.Id == taskId && t.UserId == userId);
            if (task == null) return new List<TaskHistoryDto>();
        }

        var history = await query
            .OrderByDescending(h => h.CreatedAt)
            .ToListAsync();

        return history.Select(h => new TaskHistoryDto
        {
            Id = h.Id,
            TaskId = h.TaskId,
            UserId = h.UserId,
            UserName = h.User.Email,
            Action = h.Action,
            OldValue = h.OldValue,
            NewValue = h.NewValue,
            Details = h.Details,
            CreatedAt = h.CreatedAt
        }).ToList();
    }

    public async Task<byte[]> ExportTasksToPdfAsync(int userId, bool isAdmin)
    {
        var tasks = await GetTasksAsync(userId, isAdmin);

        using var memoryStream = new MemoryStream();
        var pdfWriter = new PdfWriter(memoryStream);
        var pdfDocument = new PdfDocument(pdfWriter);
        var document = new Document(pdfDocument);

        // Título
        var title = new Paragraph("Reporte de Tareas - KanbanFlow")
            .SetFontSize(20)
            .SetTextAlignment(TextAlignment.CENTER)
            .SetMarginBottom(20);
        document.Add(title);

        // Fecha de generación
        var date = new Paragraph($"Generado el: {DateTime.Now:dd/MM/yyyy HH:mm}")
            .SetFontSize(10)
            .SetTextAlignment(TextAlignment.RIGHT)
            .SetMarginBottom(20);
        document.Add(date);

        // Crear tabla
        var table = new Table(UnitValue.CreatePercentArray(new float[] { 2, 2, 1, 1, 1, 1 }))
            .SetWidth(UnitValue.CreatePercentValue(100))
            .SetMarginBottom(20);

        // Encabezados
        table.AddHeaderCell(new Cell().Add(new Paragraph("Título").SetFontSize(10).SetBold()));
        table.AddHeaderCell(new Cell().Add(new Paragraph("Descripción").SetFontSize(10).SetBold()));
        table.AddHeaderCell(new Cell().Add(new Paragraph("Estado").SetFontSize(10).SetBold()));
        table.AddHeaderCell(new Cell().Add(new Paragraph("Prioridad").SetFontSize(10).SetBold()));
        table.AddHeaderCell(new Cell().Add(new Paragraph("Fecha límite").SetFontSize(10).SetBold()));
        table.AddHeaderCell(new Cell().Add(new Paragraph("Tags").SetFontSize(10).SetBold()));

        // Datos
        foreach (var task in tasks)
        {
            table.AddCell(new Cell().Add(new Paragraph(task.Title ?? "").SetFontSize(9)));
            table.AddCell(new Cell().Add(new Paragraph(task.Description ?? "").SetFontSize(9)));
            table.AddCell(new Cell().Add(new Paragraph(GetStatusLabel(task.Status)).SetFontSize(9)));
            table.AddCell(new Cell().Add(new Paragraph(GetPriorityLabel(task.Priority)).SetFontSize(9)));
            table.AddCell(new Cell().Add(new Paragraph(task.DueDate?.ToString("dd/MM/yyyy") ?? "Sin fecha").SetFontSize(9)));
            table.AddCell(new Cell().Add(new Paragraph(task.Tags ?? "Sin tags").SetFontSize(9)));
        }

        document.Add(table);
        document.Close();

        return memoryStream.ToArray();
    }

    public async Task<byte[]> ExportTasksToExcelAsync(int userId, bool isAdmin)
    {
        var tasks = await GetTasksAsync(userId, isAdmin);

        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add("Tareas");

        // Encabezados
        worksheet.Cell(1, 1).Value = "Título";
        worksheet.Cell(1, 2).Value = "Descripción";
        worksheet.Cell(1, 3).Value = "Estado";
        worksheet.Cell(1, 4).Value = "Prioridad";
        worksheet.Cell(1, 5).Value = "Fecha límite";
        worksheet.Cell(1, 6).Value = "Tags";
        worksheet.Cell(1, 7).Value = "Fecha creación";
        worksheet.Cell(1, 8).Value = "Última actualización";

        // Estilo de encabezados
        var headerRange = worksheet.Range("A1:H1");
        headerRange.Style.Font.Bold = true;
        headerRange.Style.Fill.BackgroundColor = XLColor.LightGray;

        // Datos
        for (int i = 0; i < tasks.Count; i++)
        {
            var task = tasks[i];
            var row = i + 2;

            worksheet.Cell(row, 1).Value = task.Title ?? "";
            worksheet.Cell(row, 2).Value = task.Description ?? "";
            worksheet.Cell(row, 3).Value = GetStatusLabel(task.Status);
            worksheet.Cell(row, 4).Value = GetPriorityLabel(task.Priority);
            worksheet.Cell(row, 5).Value = task.DueDate?.ToString("dd/MM/yyyy") ?? "";
            worksheet.Cell(row, 6).Value = task.Tags ?? "";
            worksheet.Cell(row, 7).Value = task.CreatedAt.ToString("dd/MM/yyyy HH:mm");
            worksheet.Cell(row, 8).Value = task.UpdatedAt.ToString("dd/MM/yyyy HH:mm");
        }

        // Autoajustar columnas
        worksheet.Columns().AdjustToContents();

        using var memoryStream = new MemoryStream();
        workbook.SaveAs(memoryStream);

        return memoryStream.ToArray();
    }

    // Mantener el método existente por compatibilidad
    public async Task<byte[]> ExportTasksAsync(int userId, bool isAdmin)
    {
        return await ExportTasksToExcelAsync(userId, isAdmin);
    }

    private string GetStatusLabel(Kanban.Core.Entities.TaskStatus status)
    {
        return status switch
        {
            Kanban.Core.Entities.TaskStatus.ToDo => "Por hacer",
            Kanban.Core.Entities.TaskStatus.InProgress => "En progreso",
            Kanban.Core.Entities.TaskStatus.Done => "Completada",
            _ => "Desconocido"
        };
    }

    private string GetPriorityLabel(TaskPriority priority)
    {
        return priority switch
        {
            TaskPriority.Low => "Baja",
            TaskPriority.Medium => "Media",
            TaskPriority.High => "Alta",
            TaskPriority.Urgent => "Urgente",
            _ => "Desconocida"
        };
    }

    private TaskDto MapToDto(Kanban.Core.Entities.Task task)
    {
        return new TaskDto
        {
            Id = task.Id,
            Title = task.Title,
            Description = task.Description,
            Status = task.Status,
            Position = task.Position,
            UserId = task.UserId,
            UserName = task.User.Email,
            AssignedToUserId = task.AssignedToUserId,
            AssignedToUserName = task.AssignedToUser?.Email,
            DueDate = task.DueDate,
            Priority = task.Priority,
            Tags = task.Tags,
            Comments = task.Comments,
            CreatedAt = task.CreatedAt,
            UpdatedAt = task.UpdatedAt
        };
    }

    private async System.Threading.Tasks.Task AdjustPositionsAsync(Kanban.Core.Entities.Task task, Kanban.Core.Entities.TaskStatus newStatus, int newPosition, int userId)
    {
        if (task.Status != newStatus)
        {
            // Ajustar posiciones en el estado anterior
            var tasksInOldStatus = await _context.Tasks
                .Where(t => t.UserId == userId && t.Status == task.Status && t.Id != task.Id)
                .ToListAsync();

            foreach (var t in tasksInOldStatus.Where(t => t.Position > task.Position))
            {
                t.Position--;
            }

            // Ajustar posiciones en el nuevo estado
            var tasksInNewStatus = await _context.Tasks
                .Where(t => t.UserId == userId && t.Status == newStatus)
                .ToListAsync();

            foreach (var t in tasksInNewStatus.Where(t => t.Position >= newPosition))
            {
                t.Position++;
            }
        }
        else
        {
            // Misma columna, ajustar posiciones
            var tasks = await _context.Tasks
                .Where(t => t.UserId == userId && t.Status == newStatus && t.Id != task.Id)
                .ToListAsync();

            if (newPosition < task.Position)
            {
                foreach (var t in tasks.Where(t => t.Position >= newPosition && t.Position < task.Position))
                {
                    t.Position++;
                }
            }
            else if (newPosition > task.Position)
            {
                foreach (var t in tasks.Where(t => t.Position <= newPosition && t.Position > task.Position))
                {
                    t.Position--;
                }
            }
        }
    }

    private async System.Threading.Tasks.Task AddHistoryEntryAsync(int taskId, int userId, string action, string? oldValue, string? newValue, string? details = null)
    {
        // Get the user to set UserName
        var user = await _context.Users.FindAsync(userId);
        if (user == null)
        {
            // For special users like teacher bypass, use a default name
            var history = new TaskHistory
            {
                TaskId = taskId,
                UserId = userId,
                UserName = "Docente FESC", // Default for teacher
                Action = action,
                OldValue = oldValue,
                NewValue = newValue,
                Details = details
            };

            _context.TaskHistories.Add(history);
            await _context.SaveChangesAsync();
            return;
        }

        var historyEntry = new TaskHistory
        {
            TaskId = taskId,
            UserId = userId,
            UserName = user.Email, // Set UserName from user
            Action = action,
            OldValue = oldValue,
            NewValue = newValue,
            Details = details
        };

        _context.TaskHistories.Add(historyEntry);
        await _context.SaveChangesAsync();
    }

    private async System.Threading.Tasks.Task LogTaskChangesAsync(Kanban.Core.Entities.Task task, object oldValues, int userId)
    {
        // Comparar cambios y registrar en historial
        var changes = new List<(string field, string? oldVal, string? newVal)>();

        // Aquí irían las comparaciones de cada campo
        // Por simplicidad, registrar un cambio general
        await AddHistoryEntryAsync(task.Id, userId, "Updated", null, null, "Task was updated");
    }
}