using System.ComponentModel.DataAnnotations;

namespace Kanban.Application.DTOs
{
    public class CreateTaskDto
    {
        [Required]
        [StringLength(100, MinimumLength = 3)]
        public string Title { get; set; } = default!;

        [StringLength(500)]
        public string? Description { get; set; }

        [Required]
        [RegularExpression("ToDo|InProgress|Done")]
        public string Status { get; set; } = "ToDo";

        [Range(0, int.MaxValue)]
        public int Position { get; set; }
    }

    public class UpdateTaskDto : CreateTaskDto
    {
        [Required]
        public int Id { get; set; }
    }
}
