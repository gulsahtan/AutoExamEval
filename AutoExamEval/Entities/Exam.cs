using System.ComponentModel.DataAnnotations;
using AutoExamEval.Enums;

namespace AutoExamEval.Entities;

public class Exam
{
    public int Id { get; set; }

    [Required]
    public int CourseId { get; set; }

    [Required]
    [StringLength(200)]
    public string ExamTitle { get; set; } = string.Empty;

    [Required]
    public ExamType ExamType { get; set; }

    [Required]
    public DateTime ExamDate { get; set; }

    [Required]
    [StringLength(200)]
    public string ExamLocation { get; set; } = string.Empty;

    [Required]
    [StringLength(150)]
    public string InstructorName { get; set; } = string.Empty;

    [Range(1, int.MaxValue)]
    public int DurationMinutes { get; set; }

    [Range(typeof(decimal), "0.01", "999999")]
    public decimal TotalScore { get; set; }

    [StringLength(100)]
    public string? TemplateType { get; set; }

    [StringLength(1000)]
    public string? Description { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public Course? Course { get; set; }

    public ICollection<Question> Questions { get; set; } = new List<Question>();
}
