using AutoExamEval.Enums;

namespace AutoExamEval.ViewModels.Question;

public class QuestionListViewModel
{
    public int Id { get; set; }
    public int ExamId { get; set; }
    public string ExamTitle { get; set; } = string.Empty;
    public int QuestionNo { get; set; }
    public string QuestionText { get; set; } = string.Empty;
    public QuestionType QuestionType { get; set; }
    public decimal Score { get; set; }
}
