namespace AutoExamEval.ViewModels.AnswerImport;

public class ImportedStudentAnswerViewModel
{
    public string StudentNumber { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public int ImportedAnswerCount { get; set; }
    public string StatusMessage { get; set; } = string.Empty;
}
