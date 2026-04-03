using AutoExamEval.Data;
using AutoExamEval.Entities;
using AutoExamEval.Services.Interfaces;
using AutoExamEval.ViewModels.Course;
using Microsoft.EntityFrameworkCore;

namespace AutoExamEval.Services;

public class CourseService : ICourseService
{
    private readonly ApplicationDbContext _context;

    public CourseService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<CourseListViewModel>> GetAllAsync()
    {
        return await _context.Courses
            .AsNoTracking()
            .OrderBy(c => c.CourseCode)
            .Select(c => new CourseListViewModel
            {
                Id = c.Id,
                CourseCode = c.CourseCode,
                CourseName = c.CourseName,
                Term = c.Term,
                AcademicYear = c.AcademicYear,
                InstructorName = c.InstructorName
            })
            .ToListAsync();
    }

    public async Task<Course?> GetByIdAsync(int id)
    {
        return await _context.Courses
            .AsNoTracking()
            .Include(c => c.Exams)
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<int> CreateAsync(CourseCreateViewModel model)
    {
        var now = DateTime.UtcNow;

        var course = new Course
        {
            CourseCode = model.CourseCode.Trim(),
            CourseName = model.CourseName.Trim(),
            Term = model.Term.Trim(),
            AcademicYear = model.AcademicYear.Trim(),
            InstructorName = model.InstructorName.Trim(),
            Description = string.IsNullOrWhiteSpace(model.Description) ? null : model.Description.Trim(),
            CreatedAt = now,
            UpdatedAt = now
        };

        _context.Courses.Add(course);
        await _context.SaveChangesAsync();

        return course.Id;
    }

    public async Task<bool> UpdateAsync(CourseEditViewModel model)
    {
        var existingCourse = await _context.Courses.FirstOrDefaultAsync(c => c.Id == model.Id);
        if (existingCourse is null)
        {
            return false;
        }

        existingCourse.CourseCode = model.CourseCode.Trim();
        existingCourse.CourseName = model.CourseName.Trim();
        existingCourse.Term = model.Term.Trim();
        existingCourse.AcademicYear = model.AcademicYear.Trim();
        existingCourse.InstructorName = model.InstructorName.Trim();
        existingCourse.Description = string.IsNullOrWhiteSpace(model.Description) ? null : model.Description.Trim();
        existingCourse.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var existingCourse = await _context.Courses.Include(c => c.Exams)
            .FirstOrDefaultAsync(c => c.Id == id);
        if (existingCourse is null)
        {
            return false;
        }

        _context.Courses.Remove(existingCourse);

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
        return await _context.Courses.AnyAsync(c => c.Id == id);
    }
}
