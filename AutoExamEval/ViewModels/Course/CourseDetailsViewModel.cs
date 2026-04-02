using AutoExamEval.Enums;

namespace AutoExamEval.ViewModels.Course;

public class CourseDetailsViewModel
{
    public int Id { get; set; }
    public string CourseCode { get; set; } = string.Empty;
    public string CourseName { get; set; } = string.Empty;
    public string Term { get; set; } = string.Empty;
    public string AcademicYear { get; set; } = string.Empty;
    public string InstructorName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public List<CourseExamListItemViewModel> Exams { get; set; } = new();
}

public class CourseExamListItemViewModel
{
    public int Id { get; set; }
    public string ExamTitle { get; set; } = string.Empty;
    public ExamType ExamType { get; set; }
    public DateTime ExamDate { get; set; }
    public int DurationMinutes { get; set; }
    public decimal TotalScore { get; set; }
}
