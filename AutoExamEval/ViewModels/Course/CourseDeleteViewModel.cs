namespace AutoExamEval.ViewModels.Course;

public class CourseDeleteViewModel
{
    public int Id { get; set; }
    public string CourseCode { get; set; } = string.Empty;
    public string CourseName { get; set; } = string.Empty;
    public string Term { get; set; } = string.Empty;
    public string AcademicYear { get; set; } = string.Empty;
    public string InstructorName { get; set; } = string.Empty;
}
