using AutoExamEval.Entities;
using AutoExamEval.ViewModels.AnswerImport;

namespace AutoExamEval.Services.Interfaces;

public interface IAnswerImportService
{
    Task<AnswerImportResultViewModel> ImportFromCsvAsync(AnswerImportCsvViewModel model, string? userId);
    Task<ManualAnswerEntryViewModel?> GetManualEntryViewModelAsync(int examId);
    Task<AnswerImportResultViewModel> SaveManualAnswersAsync(ManualAnswerEntryViewModel model, string? userId);
    Task<AnswerImportResultViewModel?> GetBatchDetailsAsync(int batchId);
    Task<List<OpticalReadBatch>> GetExamBatchesAsync(int examId);
}
