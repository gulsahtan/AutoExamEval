using System.ComponentModel.DataAnnotations;
using AutoExamEval.Enums;

namespace AutoExamEval.Entities;

public class OpticalReadBatch
{
    public int Id { get; set; }

    [Required]
    public int ExamId { get; set; }

    [Required]
    [StringLength(150)]
    public string BatchName { get; set; } = string.Empty;

    public DateTime ImportedAt { get; set; }

    [StringLength(450)]
    public string? ImportedByUserId { get; set; }

    [Required]
    public ImportSourceType SourceType { get; set; }

    [StringLength(260)]
    public string? FileName { get; set; }

    [StringLength(1000)]
    public string? Notes { get; set; }

    public int TotalRecordCount { get; set; }

    public int SuccessfulRecordCount { get; set; }

    public int FailedRecordCount { get; set; }

    public Exam? Exam { get; set; }

    public ICollection<StudentAnswer> StudentAnswers { get; set; } = new List<StudentAnswer>();
}
