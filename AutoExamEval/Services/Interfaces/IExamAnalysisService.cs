using AutoExamEval.ViewModels.Analysis;

namespace AutoExamEval.Services.Interfaces;

public interface IExamAnalysisService
{
    Task<ExamAnalysisSummaryViewModel?> GetExamSummaryAsync(int examId);
    Task<List<StudentPerformanceViewModel>?> GetStudentPerformancesAsync(int examId);
    Task<List<QuestionPerformanceViewModel>?> GetQuestionPerformancesAsync(int examId);
    Task<List<LearningOutcomePerformanceViewModel>?> GetLearningOutcomePerformancesAsync(int examId);
    Task<ExamFullAnalysisViewModel?> GetFullAnalysisAsync(int examId);
}
