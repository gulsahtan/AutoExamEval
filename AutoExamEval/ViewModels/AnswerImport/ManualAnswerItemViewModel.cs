using AutoExamEval.Enums;

namespace AutoExamEval.ViewModels.AnswerImport;

public class ManualAnswerItemViewModel
{
    public int QuestionId { get; set; }
    public int QuestionNo { get; set; }
    public string QuestionText { get; set; } = string.Empty;
    public QuestionType QuestionType { get; set; }
    public string? GivenAnswer { get; set; }
}
