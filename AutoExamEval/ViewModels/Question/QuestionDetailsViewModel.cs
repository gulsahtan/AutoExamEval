using AutoExamEval.ViewModels.QuestionOutcome;
using AutoExamEval.Enums;

namespace AutoExamEval.ViewModels.Question;

public class QuestionDetailsViewModel
{
    public int Id { get; set; }
    public int ExamId { get; set; }
    public string ExamTitle { get; set; } = string.Empty;
    public string CourseName { get; set; } = string.Empty;
    public int QuestionNo { get; set; }
    public string QuestionText { get; set; } = string.Empty;
    public QuestionType QuestionType { get; set; }
    public string? OptionA { get; set; }
    public string? OptionB { get; set; }
    public string? OptionC { get; set; }
    public string? OptionD { get; set; }
    public string? OptionE { get; set; }
    public string? CorrectAnswer { get; set; }
    public decimal Score { get; set; }
    public string? AnswerText { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public List<QuestionOutcomeListViewModel> AssignedOutcomes { get; set; } = new();
}
