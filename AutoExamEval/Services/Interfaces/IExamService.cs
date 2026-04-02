using AutoExamEval.Entities;
using AutoExamEval.ViewModels.Exam;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace AutoExamEval.Services.Interfaces;

public interface IExamService
{
    Task<List<ExamListViewModel>> GetAllAsync();
    Task<Exam?> GetByIdAsync(int id);
    Task<List<ExamListViewModel>> GetByCourseIdAsync(int courseId);
    Task<int> CreateAsync(ExamCreateViewModel model);
    Task<bool> UpdateAsync(ExamEditViewModel model);
    Task<bool> DeleteAsync(int id);
    Task<bool> ExistsAsync(int id);
    Task<List<SelectListItem>> GetCourseSelectListAsync();
    List<SelectListItem> GetExamTypeSelectList();
}
