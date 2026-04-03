using AutoExamEval.ViewModels.QuestionOutcome;

namespace AutoExamEval.Services.Interfaces;

public interface IQuestionOutcomeService
{
    Task<List<QuestionOutcomeListViewModel>> GetAssignmentsByQuestionIdAsync(int questionId);
    Task<QuestionOutcomeAssignViewModel?> GetAssignViewModelAsync(int questionId);
    Task SaveAssignmentsAsync(QuestionOutcomeAssignViewModel model);
    Task DeleteAssignmentsByQuestionIdAsync(int questionId);
    Task<bool> QuestionExistsAsync(int questionId);
}
