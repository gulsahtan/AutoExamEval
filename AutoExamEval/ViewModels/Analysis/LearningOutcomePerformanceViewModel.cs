namespace AutoExamEval.ViewModels.Analysis;

public class LearningOutcomePerformanceViewModel
{
    public int LearningOutcomeId { get; set; }
    public string OutcomeCode { get; set; } = string.Empty;
    public string OutcomeDescription { get; set; } = string.Empty;
    public int RelatedQuestionCount { get; set; }
    public int StudentInteractionCount { get; set; }
    public decimal SuccessRate { get; set; }
    public decimal AverageContributionScore { get; set; }
}
