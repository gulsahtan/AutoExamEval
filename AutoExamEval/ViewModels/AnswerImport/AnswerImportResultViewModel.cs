namespace AutoExamEval.ViewModels.AnswerImport;

public class AnswerImportResultViewModel
{
    public int ExamId { get; set; }
    public string ExamTitle { get; set; } = string.Empty;
    public int BatchId { get; set; }
    public string BatchName { get; set; } = string.Empty;
    public int TotalRecordCount { get; set; }
    public int SuccessfulRecordCount { get; set; }
    public int FailedRecordCount { get; set; }
    public List<string> Errors { get; set; } = new();
    public List<ImportedStudentAnswerViewModel> ImportedRowsSummary { get; set; } = new();
}
