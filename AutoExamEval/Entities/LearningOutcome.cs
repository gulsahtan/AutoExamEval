using System.ComponentModel.DataAnnotations;

namespace AutoExamEval.Entities;

public class LearningOutcome
{
    public int Id { get; set; }

    [Required]
    public int CourseId { get; set; }

    [Required]
    [StringLength(50)]
    public string OutcomeCode { get; set; } = string.Empty;

    [Required]
    [StringLength(1000)]
    public string Description { get; set; } = string.Empty;

    public decimal? Weight { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public Course? Course { get; set; }

    public ICollection<QuestionOutcome> QuestionOutcomes { get; set; } = new List<QuestionOutcome>();
}
