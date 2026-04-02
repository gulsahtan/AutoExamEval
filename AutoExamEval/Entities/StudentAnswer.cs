using System.ComponentModel.DataAnnotations;

namespace AutoExamEval.Entities;

public class StudentAnswer
{
    public int Id { get; set; }

    [Required]
    public int ExamId { get; set; }

    [Required]
    public int StudentId { get; set; }

    [Required]
    public int QuestionId { get; set; }

    public int? OpticalReadBatchId { get; set; }

    [StringLength(200)]
    public string? GivenAnswer { get; set; }

    public bool IsImported { get; set; }

    public DateTime? ImportedAt { get; set; }

    [StringLength(500)]
    public string? RawValue { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public Exam? Exam { get; set; }

    public Student? Student { get; set; }

    public Question? Question { get; set; }

    public OpticalReadBatch? OpticalReadBatch { get; set; }
}
