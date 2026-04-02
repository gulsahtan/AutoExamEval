namespace AutoExamEval.ViewModels.Analysis;

public class StudentPerformanceViewModel
{
    public int StudentId { get; set; }
    public string StudentNumber { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public int CorrectCount { get; set; }
    public int WrongCount { get; set; }
    public int BlankCount { get; set; }
    public decimal TotalScore { get; set; }
    public decimal SuccessRate { get; set; }
    public bool NeedsManualEvaluation { get; set; }
}
