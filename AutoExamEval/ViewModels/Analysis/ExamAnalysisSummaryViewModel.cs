namespace AutoExamEval.ViewModels.Analysis;

public class ExamAnalysisSummaryViewModel
{
    public int ExamId { get; set; }
    public string ExamTitle { get; set; } = string.Empty;
    public string CourseName { get; set; } = string.Empty;
    public int TotalStudents { get; set; }
    public int TotalQuestions { get; set; }
    public int TotalAnswers { get; set; }
    public decimal AverageScore { get; set; }
    public decimal HighestScore { get; set; }
    public decimal LowestScore { get; set; }
    public decimal AverageSuccessRate { get; set; }
}
