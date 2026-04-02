using AutoExamEval.Services.Interfaces;
using AutoExamEval.ViewModels.Exam;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AutoExamEval.Controllers;

[Authorize(Roles = "Admin")]
public class ExamController : Controller
{
    private readonly IExamService _examService;
    private readonly ICourseService _courseService;

    public ExamController(IExamService examService, ICourseService courseService)
    {
        _examService = examService;
        _courseService = courseService;
    }

    public async Task<IActionResult> Index()
    {
        var exams = await _examService.GetAllAsync();
        return View(exams);
    }

    public async Task<IActionResult> ByCourse(int courseId)
    {
        var course = await _courseService.GetByIdAsync(courseId);
        if (course is null)
        {
            return NotFound();
        }

        var exams = await _examService.GetByCourseIdAsync(courseId);
        ViewBag.CourseId = courseId;
        ViewBag.CourseName = $"{course.CourseCode} - {course.CourseName}";

        return View(exams);
    }

    public async Task<IActionResult> Details(int id)
    {
        var exam = await _examService.GetByIdAsync(id);
        if (exam is null)
        {
            return NotFound();
        }

        var model = new ExamDetailsViewModel
        {
            Id = exam.Id,
            CourseId = exam.CourseId,
            CourseName = exam.Course != null ? $"{exam.Course.CourseCode} - {exam.Course.CourseName}" : "-",
            ExamTitle = exam.ExamTitle,
            ExamType = exam.ExamType,
            ExamDate = exam.ExamDate,
            ExamLocation = exam.ExamLocation,
            InstructorName = exam.InstructorName,
            DurationMinutes = exam.DurationMinutes,
            TotalScore = exam.TotalScore,
            TemplateType = exam.TemplateType,
            Description = exam.Description,
            CreatedAt = exam.CreatedAt,
            UpdatedAt = exam.UpdatedAt,
            Questions = exam.Questions
                .OrderBy(x => x.QuestionNo)
                .Select(x => new ExamQuestionListItemViewModel
                {
                    Id = x.Id,
                    QuestionNo = x.QuestionNo,
                    QuestionType = x.QuestionType,
                    QuestionText = x.QuestionText,
                    Score = x.Score
                })
                .ToList()
        };

        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> Create(int? courseId)
    {
        var model = new ExamCreateViewModel
        {
            CourseId = courseId ?? 0,
            ExamDate = DateTime.Today
        };

        await PopulateSelectionsAsync(model);
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ExamCreateViewModel model)
    {
        if (!ModelState.IsValid)
        {
            await PopulateSelectionsAsync(model);
            return View(model);
        }

        var examId = await _examService.CreateAsync(model);
        TempData["SuccessMessage"] = "Exam başarıyla oluşturuldu.";

        return RedirectToAction(nameof(ByCourse), new { courseId = model.CourseId, createdExamId = examId });
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var exam = await _examService.GetByIdAsync(id);
        if (exam is null)
        {
            return NotFound();
        }

        var model = new ExamEditViewModel
        {
            Id = exam.Id,
            CourseId = exam.CourseId,
            CourseName = exam.Course != null ? $"{exam.Course.CourseCode} - {exam.Course.CourseName}" : null,
            ExamTitle = exam.ExamTitle,
            ExamType = exam.ExamType,
            ExamDate = exam.ExamDate,
            ExamLocation = exam.ExamLocation,
            InstructorName = exam.InstructorName,
            DurationMinutes = exam.DurationMinutes,
            TotalScore = exam.TotalScore,
            TemplateType = exam.TemplateType,
            Description = exam.Description
        };

        await PopulateSelectionsAsync(model);
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, ExamEditViewModel model)
    {
        if (id != model.Id)
        {
            return NotFound();
        }

        if (!ModelState.IsValid)
        {
            await PopulateSelectionsAsync(model);
            return View(model);
        }

        var updated = await _examService.UpdateAsync(model);
        if (!updated)
        {
            return NotFound();
        }

        TempData["SuccessMessage"] = "Exam başarıyla güncellendi.";
        return RedirectToAction(nameof(ByCourse), new { courseId = model.CourseId });
    }

    [HttpGet]
    public async Task<IActionResult> Delete(int id)
    {
        var exam = await _examService.GetByIdAsync(id);
        if (exam is null)
        {
            return NotFound();
        }

        var model = new ExamDeleteViewModel
        {
            Id = exam.Id,
            CourseId = exam.CourseId,
            CourseName = exam.Course != null ? $"{exam.Course.CourseCode} - {exam.Course.CourseName}" : "-",
            ExamTitle = exam.ExamTitle,
            ExamType = exam.ExamType,
            ExamDate = exam.ExamDate,
            DurationMinutes = exam.DurationMinutes,
            TotalScore = exam.TotalScore
        };

        return View(model);
    }

    [HttpPost]
    [ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id, int courseId)
    {
        var deleted = await _examService.DeleteAsync(id);
        if (!deleted)
        {
            TempData["ErrorMessage"] = "Exam silinemedi. İlişkili soruları kontrol ediniz.";
            return RedirectToAction(nameof(ByCourse), new { courseId });
        }

        TempData["SuccessMessage"] = "Exam başarıyla silindi.";
        return RedirectToAction(nameof(ByCourse), new { courseId });
    }

    private async Task PopulateSelectionsAsync(ExamCreateViewModel model)
    {
        model.CourseOptions = await _examService.GetCourseSelectListAsync();
        model.ExamTypeOptions = _examService.GetExamTypeSelectList();
    }

    private async Task PopulateSelectionsAsync(ExamEditViewModel model)
    {
        model.CourseOptions = await _examService.GetCourseSelectListAsync();
        model.ExamTypeOptions = _examService.GetExamTypeSelectList();
    }
}
