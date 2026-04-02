using AutoExamEval.Services.Interfaces;
using AutoExamEval.ViewModels.Question;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AutoExamEval.Controllers;

[Authorize(Roles = "Admin")]
public class QuestionController : Controller
{
    private readonly IQuestionService _questionService;
    private readonly IExamService _examService;

    public QuestionController(IQuestionService questionService, IExamService examService)
    {
        _questionService = questionService;
        _examService = examService;
    }

    public async Task<IActionResult> Index()
    {
        var questions = await _questionService.GetAllAsync();
        return View(questions);
    }

    public async Task<IActionResult> ByExam(int examId)
    {
        var exam = await _examService.GetByIdAsync(examId);
        if (exam is null)
        {
            return NotFound();
        }

        var questions = await _questionService.GetByExamIdAsync(examId);
        ViewBag.ExamId = examId;
        ViewBag.ExamTitle = exam.ExamTitle;
        ViewBag.CourseName = exam.Course != null ? $"{exam.Course.CourseCode} - {exam.Course.CourseName}" : "-";

        return View(questions);
    }

    public async Task<IActionResult> Details(int id)
    {
        var question = await _questionService.GetByIdAsync(id);
        if (question is null)
        {
            return NotFound();
        }

        var model = new QuestionDetailsViewModel
        {
            Id = question.Id,
            ExamId = question.ExamId,
            ExamTitle = question.Exam?.ExamTitle ?? "-",
            CourseName = question.Exam?.Course != null
                ? $"{question.Exam.Course.CourseCode} - {question.Exam.Course.CourseName}"
                : "-",
            QuestionNo = question.QuestionNo,
            QuestionText = question.QuestionText,
            QuestionType = question.QuestionType,
            OptionA = question.OptionA,
            OptionB = question.OptionB,
            OptionC = question.OptionC,
            OptionD = question.OptionD,
            OptionE = question.OptionE,
            CorrectAnswer = question.CorrectAnswer,
            Score = question.Score,
            AnswerText = question.AnswerText,
            CreatedAt = question.CreatedAt,
            UpdatedAt = question.UpdatedAt
        };

        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> Create(int? examId)
    {
        var model = new QuestionCreateViewModel();

        if (examId.HasValue)
        {
            var exam = await _examService.GetByIdAsync(examId.Value);
            if (exam is null)
            {
                return NotFound();
            }

            model.ExamId = examId.Value;
            model.ExamTitle = exam.ExamTitle;
            model.CourseName = exam.Course != null ? $"{exam.Course.CourseCode} - {exam.Course.CourseName}" : "-";
            model.QuestionNo = await _questionService.GetNextQuestionNumberAsync(examId.Value);
        }

        await PopulateSelectionsAsync(model);
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(QuestionCreateViewModel model)
    {
        if (!ModelState.IsValid)
        {
            await PopulateSelectionsAsync(model);
            return View(model);
        }

        try
        {
            await _questionService.CreateAsync(model);
        }
        catch (InvalidOperationException ex)
        {
            ModelState.AddModelError(nameof(model.QuestionNo), ex.Message);
            await PopulateSelectionsAsync(model);
            return View(model);
        }

        TempData["SuccessMessage"] = "Question başarıyla oluşturuldu.";
        return RedirectToAction(nameof(ByExam), new { examId = model.ExamId });
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var question = await _questionService.GetByIdAsync(id);
        if (question is null)
        {
            return NotFound();
        }

        var model = new QuestionEditViewModel
        {
            Id = question.Id,
            ExamId = question.ExamId,
            ExamTitle = question.Exam?.ExamTitle,
            CourseName = question.Exam?.Course != null
                ? $"{question.Exam.Course.CourseCode} - {question.Exam.Course.CourseName}"
                : null,
            QuestionNo = question.QuestionNo,
            QuestionText = question.QuestionText,
            QuestionType = question.QuestionType,
            OptionA = question.OptionA,
            OptionB = question.OptionB,
            OptionC = question.OptionC,
            OptionD = question.OptionD,
            OptionE = question.OptionE,
            CorrectAnswer = question.CorrectAnswer,
            Score = question.Score,
            AnswerText = question.AnswerText
        };

        await PopulateSelectionsAsync(model);
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, QuestionEditViewModel model)
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

        try
        {
            var updated = await _questionService.UpdateAsync(model);
            if (!updated)
            {
                return NotFound();
            }
        }
        catch (InvalidOperationException ex)
        {
            ModelState.AddModelError(nameof(model.QuestionNo), ex.Message);
            await PopulateSelectionsAsync(model);
            return View(model);
        }

        TempData["SuccessMessage"] = "Question başarıyla güncellendi.";
        return RedirectToAction(nameof(ByExam), new { examId = model.ExamId });
    }

    [HttpGet]
    public async Task<IActionResult> Delete(int id)
    {
        var question = await _questionService.GetByIdAsync(id);
        if (question is null)
        {
            return NotFound();
        }

        var model = new QuestionDeleteViewModel
        {
            Id = question.Id,
            ExamId = question.ExamId,
            ExamTitle = question.Exam?.ExamTitle ?? "-",
            QuestionNo = question.QuestionNo,
            QuestionText = question.QuestionText,
            QuestionType = question.QuestionType,
            Score = question.Score
        };

        return View(model);
    }

    [HttpPost]
    [ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id, int examId)
    {
        var deleted = await _questionService.DeleteAsync(id);
        if (!deleted)
        {
            return NotFound();
        }

        TempData["SuccessMessage"] = "Question başarıyla silindi.";
        return RedirectToAction(nameof(ByExam), new { examId });
    }

    private async Task PopulateSelectionsAsync(QuestionCreateViewModel model)
    {
        model.ExamOptions = await _questionService.GetExamSelectListAsync();
        model.QuestionTypeOptions = _questionService.GetQuestionTypeSelectList();

        if (model.ExamId > 0 && !model.QuestionNo.HasValue)
        {
            model.QuestionNo = await _questionService.GetNextQuestionNumberAsync(model.ExamId);
        }
    }

    private async Task PopulateSelectionsAsync(QuestionEditViewModel model)
    {
        model.ExamOptions = await _questionService.GetExamSelectListAsync();
        model.QuestionTypeOptions = _questionService.GetQuestionTypeSelectList();
    }
}
