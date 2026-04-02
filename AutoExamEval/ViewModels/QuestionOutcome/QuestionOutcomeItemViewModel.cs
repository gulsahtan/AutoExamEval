using System.ComponentModel.DataAnnotations;

namespace AutoExamEval.ViewModels.QuestionOutcome;

public class QuestionOutcomeItemViewModel
{
    public int LearningOutcomeId { get; set; }
    public string OutcomeCode { get; set; } = string.Empty;
    public string OutcomeDescription { get; set; } = string.Empty;
    public bool IsSelected { get; set; }

    [Range(typeof(decimal), "0", "999999", ErrorMessage = "Weight negatif olamaz.")]
    public decimal? Weight { get; set; }
}
