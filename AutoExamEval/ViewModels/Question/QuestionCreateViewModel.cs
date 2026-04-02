using System.ComponentModel.DataAnnotations;
using AutoExamEval.Enums;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace AutoExamEval.ViewModels.Question;

public class QuestionCreateViewModel : IValidatableObject
{
    [Required(ErrorMessage = "Exam alanı zorunludur.")]
    [Display(Name = "Exam")]
    public int ExamId { get; set; }

    public string? ExamTitle { get; set; }
    public string? CourseName { get; set; }

    [Required(ErrorMessage = "Question No alanı zorunludur.")]
    [Range(1, int.MaxValue, ErrorMessage = "Question No 0'dan büyük olmalıdır.")]
    [Display(Name = "Question No")]
    public int? QuestionNo { get; set; }

    [Required(ErrorMessage = "Question Text alanı zorunludur.")]
    [StringLength(4000, ErrorMessage = "Question Text en fazla 4000 karakter olabilir.")]
    [Display(Name = "Question Text")]
    public string QuestionText { get; set; } = string.Empty;

    [Required(ErrorMessage = "Question Type alanı zorunludur.")]
    [Display(Name = "Question Type")]
    public QuestionType? QuestionType { get; set; }

    [StringLength(1000)]
    [Display(Name = "Option A")]
    public string? OptionA { get; set; }

    [StringLength(1000)]
    [Display(Name = "Option B")]
    public string? OptionB { get; set; }

    [StringLength(1000)]
    [Display(Name = "Option C")]
    public string? OptionC { get; set; }

    [StringLength(1000)]
    [Display(Name = "Option D")]
    public string? OptionD { get; set; }

    [StringLength(1000)]
    [Display(Name = "Option E")]
    public string? OptionE { get; set; }

    [StringLength(100)]
    [Display(Name = "Correct Answer")]
    public string? CorrectAnswer { get; set; }

    [Required(ErrorMessage = "Score alanı zorunludur.")]
    [Range(typeof(decimal), "0.01", "999999", ErrorMessage = "Score 0'dan büyük olmalıdır.")]
    [Display(Name = "Score")]
    public decimal? Score { get; set; }

    [StringLength(2000)]
    [Display(Name = "Answer Text")]
    public string? AnswerText { get; set; }

    public IEnumerable<SelectListItem> ExamOptions { get; set; } = Enumerable.Empty<SelectListItem>();
    public IEnumerable<SelectListItem> QuestionTypeOptions { get; set; } = Enumerable.Empty<SelectListItem>();

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (QuestionType is QuestionType.MultipleChoice)
        {
            if (string.IsNullOrWhiteSpace(OptionA))
            {
                yield return new ValidationResult("Option A zorunludur.", new[] { nameof(OptionA) });
            }
            if (string.IsNullOrWhiteSpace(OptionB))
            {
                yield return new ValidationResult("Option B zorunludur.", new[] { nameof(OptionB) });
            }
            if (string.IsNullOrWhiteSpace(OptionC))
            {
                yield return new ValidationResult("Option C zorunludur.", new[] { nameof(OptionC) });
            }
            if (string.IsNullOrWhiteSpace(OptionD))
            {
                yield return new ValidationResult("Option D zorunludur.", new[] { nameof(OptionD) });
            }
            if (string.IsNullOrWhiteSpace(CorrectAnswer))
            {
                yield return new ValidationResult("Correct Answer zorunludur.", new[] { nameof(CorrectAnswer) });
            }
        }

        if (QuestionType is QuestionType.TrueFalse && string.IsNullOrWhiteSpace(CorrectAnswer))
        {
            yield return new ValidationResult("True/False soruları için Correct Answer zorunludur.", new[] { nameof(CorrectAnswer) });
        }

        if (QuestionType is QuestionType.FillInTheBlank && string.IsNullOrWhiteSpace(CorrectAnswer) && string.IsNullOrWhiteSpace(AnswerText))
        {
            yield return new ValidationResult("FillInTheBlank soruları için CorrectAnswer veya AnswerText girilmelidir.", new[] { nameof(CorrectAnswer), nameof(AnswerText) });
        }
    }
}
