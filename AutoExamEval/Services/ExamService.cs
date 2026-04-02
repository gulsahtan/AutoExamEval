using AutoExamEval.Data;
using AutoExamEval.Entities;
using AutoExamEval.Services.Interfaces;
using AutoExamEval.ViewModels.Exam;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace AutoExamEval.Services;

public class ExamService : IExamService
{
    private readonly ApplicationDbContext _context;

    public ExamService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<ExamListViewModel>> GetAllAsync()
    {
        return await _context.Exams
            .AsNoTracking()
            .Include(x => x.Course)
            .OrderByDescending(x => x.ExamDate)
            .ThenBy(x => x.ExamTitle)
            .Select(x => new ExamListViewModel
            {
                Id = x.Id,
                CourseId = x.CourseId,
                CourseName = x.Course != null ? x.Course.CourseCode + " - " + x.Course.CourseName : "-",
                ExamTitle = x.ExamTitle,
                ExamType = x.ExamType,
                ExamDate = x.ExamDate,
                ExamLocation = x.ExamLocation,
                InstructorName = x.InstructorName,
                DurationMinutes = x.DurationMinutes,
                TotalScore = x.TotalScore
            })
            .ToListAsync();
    }

    public async Task<Exam?> GetByIdAsync(int id)
    {
        return await _context.Exams
            .AsNoTracking()
            .Include(x => x.Course)
            .Include(x => x.Questions)
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<List<ExamListViewModel>> GetByCourseIdAsync(int courseId)
    {
        return await _context.Exams
            .AsNoTracking()
            .Include(x => x.Course)
            .Where(x => x.CourseId == courseId)
            .OrderByDescending(x => x.ExamDate)
            .ThenBy(x => x.ExamTitle)
            .Select(x => new ExamListViewModel
            {
                Id = x.Id,
                CourseId = x.CourseId,
                CourseName = x.Course != null ? x.Course.CourseCode + " - " + x.Course.CourseName : "-",
                ExamTitle = x.ExamTitle,
                ExamType = x.ExamType,
                ExamDate = x.ExamDate,
                ExamLocation = x.ExamLocation,
                InstructorName = x.InstructorName,
                DurationMinutes = x.DurationMinutes,
                TotalScore = x.TotalScore
            })
            .ToListAsync();
    }

    public async Task<int> CreateAsync(ExamCreateViewModel model)
    {
        var now = DateTime.UtcNow;

        var exam = new Exam
        {
            CourseId = model.CourseId,
            ExamTitle = model.ExamTitle.Trim(),
            ExamType = model.ExamType!.Value,
            ExamDate = model.ExamDate!.Value,
            ExamLocation = model.ExamLocation.Trim(),
            InstructorName = model.InstructorName.Trim(),
            DurationMinutes = model.DurationMinutes!.Value,
            TotalScore = model.TotalScore!.Value,
            TemplateType = string.IsNullOrWhiteSpace(model.TemplateType) ? null : model.TemplateType.Trim(),
            Description = string.IsNullOrWhiteSpace(model.Description) ? null : model.Description.Trim(),
            CreatedAt = now,
            UpdatedAt = now
        };

        _context.Exams.Add(exam);
        await _context.SaveChangesAsync();

        return exam.Id;
    }

    public async Task<bool> UpdateAsync(ExamEditViewModel model)
    {
        var existingExam = await _context.Exams.FirstOrDefaultAsync(x => x.Id == model.Id);
        if (existingExam is null)
        {
            return false;
        }

        existingExam.CourseId = model.CourseId;
        existingExam.ExamTitle = model.ExamTitle.Trim();
        existingExam.ExamType = model.ExamType!.Value;
        existingExam.ExamDate = model.ExamDate!.Value;
        existingExam.ExamLocation = model.ExamLocation.Trim();
        existingExam.InstructorName = model.InstructorName.Trim();
        existingExam.DurationMinutes = model.DurationMinutes!.Value;
        existingExam.TotalScore = model.TotalScore!.Value;
        existingExam.TemplateType = string.IsNullOrWhiteSpace(model.TemplateType) ? null : model.TemplateType.Trim();
        existingExam.Description = string.IsNullOrWhiteSpace(model.Description) ? null : model.Description.Trim();
        existingExam.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var existingExam = await _context.Exams.FirstOrDefaultAsync(x => x.Id == id);
        if (existingExam is null)
        {
            return false;
        }

        _context.Exams.Remove(existingExam);

        try
        {
            await _context.SaveChangesAsync();
            return true;
        }
        catch (DbUpdateException)
        {
            return false;
        }
    }

    public async Task<bool> ExistsAsync(int id)
    {
        return await _context.Exams.AnyAsync(x => x.Id == id);
    }

    public async Task<List<SelectListItem>> GetCourseSelectListAsync()
    {
        return await _context.Courses
            .AsNoTracking()
            .OrderBy(x => x.CourseCode)
            .Select(x => new SelectListItem
            {
                Value = x.Id.ToString(),
                Text = x.CourseCode + " - " + x.CourseName
            })
            .ToListAsync();
    }

    public List<SelectListItem> GetExamTypeSelectList()
    {
        return Enum.GetValues<Enums.ExamType>()
            .Select(x => new SelectListItem
            {
                Value = x.ToString(),
                Text = x.ToString()
            })
            .ToList();
    }
}
