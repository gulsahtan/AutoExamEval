using AutoExamEval.Enums;

namespace AutoExamEval.ViewModels.ExamTemplate;

public class ExamTemplateViewModel
{
    public int ExamId { get; set; }
    public string ExamTitle { get; set; } = string.Empty;
    public string CourseCode { get; set; } = string.Empty;
    public string CourseName { get; set; } = string.Empty;
    public string Term { get; set; } = string.Empty;
    public string AcademicYear { get; set; } = string.Empty;
    public string InstructorName { get; set; } = string.Empty;
    public ExamType ExamType { get; set; }
    public DateTime ExamDate { get; set; }
    public string ExamLocation { get; set; } = string.Empty;
    public int DurationMinutes { get; set; }
    public decimal TotalScore { get; set; }
    public string UniversityName { get; set; } = "Üniversite Adı";
    public string FacultyName { get; set; } = "Fakülte Adı";
    public string DepartmentName { get; set; } = "Bölüm Adı";
    public string InstructionText { get; set; } = string.Empty;
    public List<ExamTemplateQuestionViewModel> Questions { get; set; } = new();
}
