using System.ComponentModel.DataAnnotations;

namespace AutoExamEval.ViewModels.Course;

public class CourseEditViewModel
{
    [Required]
    public int Id { get; set; }

    [Required(ErrorMessage = "Course Code alanı zorunludur.")]
    [StringLength(20, ErrorMessage = "Course Code en fazla 20 karakter olabilir.")]
    [Display(Name = "Course Code")]
    public string CourseCode { get; set; } = string.Empty;

    [Required(ErrorMessage = "Course Name alanı zorunludur.")]
    [StringLength(200, ErrorMessage = "Course Name en fazla 200 karakter olabilir.")]
    [Display(Name = "Course Name")]
    public string CourseName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Term alanı zorunludur.")]
    [StringLength(50, ErrorMessage = "Term en fazla 50 karakter olabilir.")]
    [Display(Name = "Term")]
    public string Term { get; set; } = string.Empty;

    [Required(ErrorMessage = "Academic Year alanı zorunludur.")]
    [StringLength(20, ErrorMessage = "Academic Year en fazla 20 karakter olabilir.")]
    [Display(Name = "Academic Year")]
    public string AcademicYear { get; set; } = string.Empty;

    [Required(ErrorMessage = "Instructor Name alanı zorunludur.")]
    [StringLength(150, ErrorMessage = "Instructor Name en fazla 150 karakter olabilir.")]
    [Display(Name = "Instructor Name")]
    public string InstructorName { get; set; } = string.Empty;

    [StringLength(1000, ErrorMessage = "Description en fazla 1000 karakter olabilir.")]
    [Display(Name = "Description")]
    public string? Description { get; set; }
}
