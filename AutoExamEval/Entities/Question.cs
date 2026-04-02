using System.ComponentModel.DataAnnotations;
using AutoExamEval.Enums;

namespace AutoExamEval.Entities;

public class Question
{
    public int Id { get; set; }

    [Required]
    public int ExamId { get; set; }

    [Range(1, int.MaxValue)]
    public int QuestionNo { get; set; }

    [Required]
    [StringLength(4000)]
    public string QuestionText { get; set; } = string.Empty;

    [Required]
    public QuestionType QuestionType { get; set; }

    [StringLength(1000)]
    public string? OptionA { get; set; }

    [StringLength(1000)]
    public string? OptionB { get; set; }

    [StringLength(1000)]
    public string? OptionC { get; set; }

    [StringLength(1000)]
    public string? OptionD { get; set; }

    [StringLength(1000)]
    public string? OptionE { get; set; }

    [StringLength(100)]
    public string? CorrectAnswer { get; set; }

    [Range(typeof(decimal), "0.01", "999999")]
    public decimal Score { get; set; }

    [StringLength(2000)]
    public string? AnswerText { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public Exam? Exam { get; set; }

    public ICollection<QuestionOutcome> QuestionOutcomes { get; set; } = new List<QuestionOutcome>();
}
