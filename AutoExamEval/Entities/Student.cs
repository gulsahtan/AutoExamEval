using System.ComponentModel.DataAnnotations;

namespace AutoExamEval.Entities;

public class Student
{
    public int Id { get; set; }

    [Required]
    [StringLength(50)]
    public string StudentNumber { get; set; } = string.Empty;

    [Required]
    [StringLength(150)]
    public string FullName { get; set; } = string.Empty;

    [StringLength(150)]
    public string? Department { get; set; }

    [StringLength(50)]
    public string? ClassName { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public ICollection<StudentAnswer> StudentAnswers { get; set; } = new List<StudentAnswer>();
}
