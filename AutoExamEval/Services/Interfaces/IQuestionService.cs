using AutoExamEval.Entities;
using AutoExamEval.ViewModels.Question;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace AutoExamEval.Services.Interfaces;

public interface IQuestionService
{
    Task<List<QuestionListViewModel>> GetAllAsync();
    Task<Question?> GetByIdAsync(int id);
    Task<List<QuestionListViewModel>> GetByExamIdAsync(int examId);
    Task<int> CreateAsync(QuestionCreateViewModel model);
    Task<bool> UpdateAsync(QuestionEditViewModel model);
    Task<bool> DeleteAsync(int id);
    Task<bool> ExistsAsync(int id);
    Task<int> GetNextQuestionNumberAsync(int examId);
    Task<List<SelectListItem>> GetExamSelectListAsync();
    List<SelectListItem> GetQuestionTypeSelectList();
}
