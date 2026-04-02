using AutoExamEval.Enums;

namespace AutoExamEval.ViewModels.Analysis;

public class QuestionPerformanceViewModel
{
    public int QuestionId { get; set; }
    public int QuestionNo { get; set; }
    public QuestionType QuestionType { get; set; }
    public string QuestionText { get; set; } = string.Empty;
    public decimal Score { get; set; }
    public int TotalResponses { get; set; }
    public int CorrectCount { get; set; }
    public int WrongCount { get; set; }
    public int BlankCount { get; set; }
    public decimal SuccessRate { get; set; }
    public decimal DifficultyIndex { get; set; }
}
