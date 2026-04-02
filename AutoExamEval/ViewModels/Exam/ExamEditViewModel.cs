using System.ComponentModel.DataAnnotations;
using AutoExamEval.Enums;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace AutoExamEval.ViewModels.Exam;

public class ExamEditViewModel
{
    [Required]
    public int Id { get; set; }

    [Required(ErrorMessage = "Course alanı zorunludur.")]
    [Display(Name = "Course")]
    public int CourseId { get; set; }

    public string? CourseName { get; set; }

    [Required(ErrorMessage = "Exam Title alanı zorunludur.")]
    [StringLength(200, ErrorMessage = "Exam Title en fazla 200 karakter olabilir.")]
    [Display(Name = "Exam Title")]
    public string ExamTitle { get; set; } = string.Empty;

    [Required(ErrorMessage = "Exam Type alanı zorunludur.")]
    [Display(Name = "Exam Type")]
    public ExamType? ExamType { get; set; }

    [Required(ErrorMessage = "Exam Date alanı zorunludur.")]
    [DataType(DataType.Date)]
    [Display(Name = "Exam Date")]
    public DateTime? ExamDate { get; set; }

    [Required(ErrorMessage = "Exam Location alanı zorunludur.")]
    [StringLength(200, ErrorMessage = "Exam Location en fazla 200 karakter olabilir.")]
    [Display(Name = "Exam Location")]
    public string ExamLocation { get; set; } = string.Empty;

    [Required(ErrorMessage = "Instructor Name alanı zorunludur.")]
    [StringLength(150, ErrorMessage = "Instructor Name en fazla 150 karakter olabilir.")]
    [Display(Name = "Instructor Name")]
    public string InstructorName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Duration alanı zorunludur.")]
    [Range(1, int.MaxValue, ErrorMessage = "Duration değeri 0'dan büyük olmalıdır.")]
    [Display(Name = "Duration (minutes)")]
    public int? DurationMinutes { get; set; }

    [Required(ErrorMessage = "Total Score alanı zorunludur.")]
    [Range(typeof(decimal), "0.01", "999999", ErrorMessage = "Total Score değeri 0'dan büyük olmalıdır.")]
    [Display(Name = "Total Score")]
    public decimal? TotalScore { get; set; }

    [StringLength(100, ErrorMessage = "Template Type en fazla 100 karakter olabilir.")]
    [Display(Name = "Template Type")]
    public string? TemplateType { get; set; }

    [StringLength(1000, ErrorMessage = "Description en fazla 1000 karakter olabilir.")]
    [Display(Name = "Description")]
    public string? Description { get; set; }

    public IEnumerable<SelectListItem> CourseOptions { get; set; } = Enumerable.Empty<SelectListItem>();
    public IEnumerable<SelectListItem> ExamTypeOptions { get; set; } = Enumerable.Empty<SelectListItem>();
}
