using AutoExamEval.ViewModels.ExamTemplate;

namespace AutoExamEval.Services.Interfaces;

public interface IExamTemplateService
{
    Task<ExamTemplateViewModel?> GetTemplateAsync(int examId);
    string BuildInstructionText(ExamTemplateViewModel model);
}
