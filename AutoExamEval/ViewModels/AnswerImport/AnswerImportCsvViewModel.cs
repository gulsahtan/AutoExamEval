using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace AutoExamEval.ViewModels.AnswerImport;

public class AnswerImportCsvViewModel
{
    [Required]
    public int ExamId { get; set; }

    public string ExamTitle { get; set; } = string.Empty;

    [Required(ErrorMessage = "CSV dosyası zorunludur.")]
    public IFormFile? CsvFile { get; set; }

    [Required(ErrorMessage = "Batch Name zorunludur.")]
    [StringLength(150)]
    public string BatchName { get; set; } = string.Empty;

    [StringLength(1000)]
    public string? Notes { get; set; }
}
