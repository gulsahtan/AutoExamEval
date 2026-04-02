namespace AutoExamEval.ViewModels.Analysis;

public class ExamFullAnalysisViewModel
{
    public ExamAnalysisSummaryViewModel Summary { get; set; } = new();
    public List<StudentPerformanceViewModel> StudentPerformances { get; set; } = new();
    public List<QuestionPerformanceViewModel> QuestionPerformances { get; set; } = new();
    public List<LearningOutcomePerformanceViewModel> LearningOutcomePerformances { get; set; } = new();
}
