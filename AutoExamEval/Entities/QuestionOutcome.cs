using System.ComponentModel.DataAnnotations;

namespace AutoExamEval.Entities;

public class QuestionOutcome
{
    public int Id { get; set; }

    [Required]
    public int QuestionId { get; set; }

    [Required]
    public int LearningOutcomeId { get; set; }

    [Range(typeof(decimal), "0", "999999")]
    public decimal? Weight { get; set; }

    public DateTime CreatedAt { get; set; }

    public Question? Question { get; set; }

    public LearningOutcome? LearningOutcome { get; set; }
}
