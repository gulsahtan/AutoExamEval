using AutoExamEval.Entities;
using AutoExamEval.ViewModels.Course;

namespace AutoExamEval.Services.Interfaces;

public interface ICourseService
{
    Task<List<CourseListViewModel>> GetAllAsync();
    Task<Course?> GetByIdAsync(int id);
    Task<int> CreateAsync(CourseCreateViewModel model);
    Task<bool> UpdateAsync(CourseEditViewModel model);
    Task<bool> DeleteAsync(int id);
    Task<bool> ExistsAsync(int id);
}
