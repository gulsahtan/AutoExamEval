using AutoExamEval.Enums;

namespace AutoExamEval.ViewModels.ExamTemplate;

public class ExamTemplateQuestionViewModel
{
    public int QuestionNo { get; set; }
    public string QuestionText { get; set; } = string.Empty;
    public QuestionType QuestionType { get; set; }
    public decimal Score { get; set; }
    public string? OptionA { get; set; }
    public string? OptionB { get; set; }
    public string? OptionC { get; set; }
    public string? OptionD { get; set; }
    public string? OptionE { get; set; }
    public bool HasOptions { get; set; }
    public bool NeedsLongAnswerArea { get; set; }
    public bool NeedsShortAnswerArea { get; set; }
    public bool ShowTrueFalseBoxes { get; set; }
}
