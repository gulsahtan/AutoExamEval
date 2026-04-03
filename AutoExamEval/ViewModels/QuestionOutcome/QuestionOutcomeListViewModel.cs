namespace AutoExamEval.ViewModels.QuestionOutcome;

public class QuestionOutcomeListViewModel
{
    public int LearningOutcomeId { get; set; }
    public string OutcomeCode { get; set; } = string.Empty;
    public string OutcomeDescription { get; set; } = string.Empty;
    public decimal? Weight { get; set; }
}
