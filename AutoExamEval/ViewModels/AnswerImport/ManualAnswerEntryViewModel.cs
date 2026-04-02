using System.ComponentModel.DataAnnotations;

namespace AutoExamEval.ViewModels.AnswerImport;

public class ManualAnswerEntryViewModel
{
    [Required]
    public int ExamId { get; set; }

    public string ExamTitle { get; set; } = string.Empty;

    public int? StudentId { get; set; }

    [Required(ErrorMessage = "Student Number zorunludur.")]
    [StringLength(50)]
    public string StudentNumber { get; set; } = string.Empty;

    [Required(ErrorMessage = "Full Name zorunludur.")]
    [StringLength(150)]
    public string FullName { get; set; } = string.Empty;

    public List<ManualAnswerItemViewModel> Answers { get; set; } = new();
}
