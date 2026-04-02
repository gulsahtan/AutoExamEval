namespace AutoExamEval.ViewModels.Student;

public class StudentListViewModel
{
    public int Id { get; set; }
    public string StudentNumber { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string? Department { get; set; }
    public string? ClassName { get; set; }
}
