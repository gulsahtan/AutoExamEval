using AutoExamEval.Enums;

namespace AutoExamEval.ViewModels.Exam;

public class ExamListViewModel
{
    public int Id { get; set; }
    public int CourseId { get; set; }
    public string CourseName { get; set; } = string.Empty;
    public string ExamTitle { get; set; } = string.Empty;
    public ExamType ExamType { get; set; }
    public DateTime ExamDate { get; set; }
    public string ExamLocation { get; set; } = string.Empty;
    public string InstructorName { get; set; } = string.Empty;
    public int DurationMinutes { get; set; }
    public decimal TotalScore { get; set; }
}
