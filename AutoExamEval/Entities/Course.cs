using System.ComponentModel.DataAnnotations;

namespace AutoExamEval.Entities;

public class Course
{
    public int Id { get; set; }

    [Required]
    [StringLength(20)]
    public string CourseCode { get; set; } = string.Empty;

    [Required]
    [StringLength(200)]
    public string CourseName { get; set; } = string.Empty;

    [Required]
    [StringLength(50)]
    public string Term { get; set; } = string.Empty;

    [Required]
    [StringLength(20)]
    public string AcademicYear { get; set; } = string.Empty;

    [Required]
    [StringLength(150)]
    public string InstructorName { get; set; } = string.Empty;

    [StringLength(1000)]
    public string? Description { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public ICollection<Exam> Exams { get; set; } = new List<Exam>();

    public ICollection<LearningOutcome> LearningOutcomes { get; set; } = new List<LearningOutcome>();
}
