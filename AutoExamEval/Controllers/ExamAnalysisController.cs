using AutoExamEval.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AutoExamEval.Controllers;

[Authorize(Roles = "Admin")]
public class ExamAnalysisController : Controller
{
    private readonly IExamAnalysisService _examAnalysisService;

    public ExamAnalysisController(IExamAnalysisService examAnalysisService)
    {
        _examAnalysisService = examAnalysisService;
    }

    public async Task<IActionResult> FullReport(int examId)
    {
        var model = await _examAnalysisService.GetFullAnalysisAsync(examId);
        if (model is null)
        {
            return NotFound();
        }

        ViewBag.HasImportedAnswers = model.Summary.TotalAnswers > 0;
        return View(model);
    }

    public async Task<IActionResult> Summary(int examId)
    {
        var model = await _examAnalysisService.GetExamSummaryAsync(examId);
        if (model is null)
        {
            return NotFound();
        }

        return View(model);
    }

    public async Task<IActionResult> Students(int examId)
    {
        var model = await _examAnalysisService.GetStudentPerformancesAsync(examId);
        if (model is null)
        {
            return NotFound();
        }

        ViewBag.ExamId = examId;
        return View(model);
    }

    public async Task<IActionResult> Questions(int examId)
    {
        var model = await _examAnalysisService.GetQuestionPerformancesAsync(examId);
        if (model is null)
        {
            return NotFound();
        }

        ViewBag.ExamId = examId;
        return View(model);
    }

    public async Task<IActionResult> Outcomes(int examId)
    {
        var model = await _examAnalysisService.GetLearningOutcomePerformancesAsync(examId);
        if (model is null)
        {
            return NotFound();
        }

        ViewBag.ExamId = examId;
        return View(model);
    }
}
