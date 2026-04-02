using System.ComponentModel.DataAnnotations;

namespace AutoExamEval.ViewModels.QuestionOutcome;

public class QuestionOutcomeAssignViewModel : IValidatableObject
{
    public int QuestionId { get; set; }
    public int QuestionNo { get; set; }
    public string QuestionText { get; set; } = string.Empty;
    public int ExamId { get; set; }
    public string ExamTitle { get; set; } = string.Empty;
    public int CourseId { get; set; }
    public string CourseName { get; set; } = string.Empty;
    public List<QuestionOutcomeItemViewModel> Outcomes { get; set; } = new();

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        var selected = Outcomes.Where(x => x.IsSelected).ToList();
        if (!selected.Any())
        {
            yield return new ValidationResult("En az bir kazanım seçmelisiniz.", new[] { nameof(Outcomes) });
        }

        foreach (var item in selected.Where(x => x.Weight.HasValue && x.Weight < 0))
        {
            yield return new ValidationResult("Weight negatif olamaz.", new[] { nameof(item.Weight) });
        }
    }
}
