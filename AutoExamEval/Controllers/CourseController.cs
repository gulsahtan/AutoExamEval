using AutoExamEval.Services.Interfaces;
using AutoExamEval.ViewModels.Course;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AutoExamEval.Controllers;

[Authorize(Roles = "Admin")]
public class CourseController : Controller
{
    private readonly ICourseService _courseService;

    public CourseController(ICourseService courseService)
    {
        _courseService = courseService;
    }

    public async Task<IActionResult> Index()
    {
        var courses = await _courseService.GetAllAsync();
        return View(courses);
    }

    public async Task<IActionResult> Details(int id)
    {
        var course = await _courseService.GetByIdAsync(id);
        if (course is null)
        {
            return NotFound();
        }

        var model = new CourseDetailsViewModel
        {
            Id = course.Id,
            CourseCode = course.CourseCode,
            CourseName = course.CourseName,
            Term = course.Term,
            AcademicYear = course.AcademicYear,
            InstructorName = course.InstructorName,
            Description = course.Description,
            CreatedAt = course.CreatedAt,
            UpdatedAt = course.UpdatedAt
        };

        return View(model);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View(new CourseCreateViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CourseCreateViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        await _courseService.CreateAsync(model);
        TempData["SuccessMessage"] = "Course başarıyla oluşturuldu.";

        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var course = await _courseService.GetByIdAsync(id);
        if (course is null)
        {
            return NotFound();
        }

        var model = new CourseEditViewModel
        {
            Id = course.Id,
            CourseCode = course.CourseCode,
            CourseName = course.CourseName,
            Term = course.Term,
            AcademicYear = course.AcademicYear,
            InstructorName = course.InstructorName,
            Description = course.Description
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, CourseEditViewModel model)
    {
        if (id != model.Id)
        {
            return NotFound();
        }

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var updated = await _courseService.UpdateAsync(model);
        if (!updated)
        {
            return NotFound();
        }

        TempData["SuccessMessage"] = "Course başarıyla güncellendi.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Delete(int id)
    {
        var course = await _courseService.GetByIdAsync(id);
        if (course is null)
        {
            return NotFound();
        }

        var model = new CourseDeleteViewModel
        {
            Id = course.Id,
            CourseCode = course.CourseCode,
            CourseName = course.CourseName,
            Term = course.Term,
            AcademicYear = course.AcademicYear,
            InstructorName = course.InstructorName
        };

        return View(model);
    }

    [HttpPost]
    [ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var deleted = await _courseService.DeleteAsync(id);
        if (!deleted)
        {
            return NotFound();
        }

        TempData["SuccessMessage"] = "Course başarıyla silindi.";
        return RedirectToAction(nameof(Index));
    }
}
