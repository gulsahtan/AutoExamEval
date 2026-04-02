using AutoExamEval.Data;
using AutoExamEval.Entities;
using AutoExamEval.Services.Interfaces;
using AutoExamEval.ViewModels.QuestionOutcome;
using Microsoft.EntityFrameworkCore;

namespace AutoExamEval.Services;

public class QuestionOutcomeService : IQuestionOutcomeService
{
    private readonly ApplicationDbContext _context;

    public QuestionOutcomeService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<QuestionOutcomeListViewModel>> GetAssignmentsByQuestionIdAsync(int questionId)
    {
        return await _context.QuestionOutcomes
            .AsNoTracking()
            .Include(x => x.LearningOutcome)
            .Where(x => x.QuestionId == questionId)
            .OrderBy(x => x.LearningOutcome!.OutcomeCode)
            .Select(x => new QuestionOutcomeListViewModel
            {
                LearningOutcomeId = x.LearningOutcomeId,
                OutcomeCode = x.LearningOutcome != null ? x.LearningOutcome.OutcomeCode : "-",
                OutcomeDescription = x.LearningOutcome != null ? x.LearningOutcome.Description : "-",
                Weight = x.Weight
            })
            .ToListAsync();
    }

    public async Task<QuestionOutcomeAssignViewModel?> GetAssignViewModelAsync(int questionId)
    {
        var question = await _context.Questions
            .AsNoTracking()
            .Include(x => x.Exam)
            .ThenInclude(x => x!.Course)
            .Include(x => x.QuestionOutcomes)
            .FirstOrDefaultAsync(x => x.Id == questionId);

        if (question is null || question.Exam is null)
        {
            return null;
        }

        var courseId = question.Exam.CourseId;
        var outcomes = await _context.LearningOutcomes
            .AsNoTracking()
            .Where(x => x.CourseId == courseId)
            .OrderBy(x => x.OutcomeCode)
            .Select(x => new { x.Id, x.OutcomeCode, x.Description })
            .ToListAsync();

        var existing = question.QuestionOutcomes.ToDictionary(x => x.LearningOutcomeId, x => x.Weight);

        return new QuestionOutcomeAssignViewModel
        {
            QuestionId = question.Id,
            QuestionNo = question.QuestionNo,
            QuestionText = question.QuestionText,
            ExamId = question.ExamId,
            ExamTitle = question.Exam.ExamTitle,
            CourseId = courseId,
            CourseName = question.Exam.Course != null
                ? $"{question.Exam.Course.CourseCode} - {question.Exam.Course.CourseName}"
                : "-",
            Outcomes = outcomes.Select(x => new QuestionOutcomeItemViewModel
            {
                LearningOutcomeId = x.Id,
                OutcomeCode = x.OutcomeCode,
                OutcomeDescription = x.Description,
                IsSelected = existing.ContainsKey(x.Id),
                Weight = existing.ContainsKey(x.Id) ? existing[x.Id] : null
            }).ToList()
        };
    }

    public async Task SaveAssignmentsAsync(QuestionOutcomeAssignViewModel model)
    {
        var question = await _context.Questions
            .AsNoTracking()
            .Include(x => x.Exam)
            .FirstOrDefaultAsync(x => x.Id == model.QuestionId);

        if (question is null || question.Exam is null)
        {
            throw new InvalidOperationException("Soru bulunamadı.");
        }

        var validOutcomeIds = await _context.LearningOutcomes
            .Where(x => x.CourseId == question.Exam.CourseId)
            .Select(x => x.Id)
            .ToListAsync();

        var selected = model.Outcomes.Where(x => x.IsSelected).ToList();
        if (!selected.Any())
        {
            throw new InvalidOperationException("En az bir kazanım seçmelisiniz.");
        }

        var invalidIds = selected
            .Where(x => !validOutcomeIds.Contains(x.LearningOutcomeId))
            .Select(x => x.LearningOutcomeId)
            .ToList();

        if (invalidIds.Any())
        {
            throw new InvalidOperationException("Sadece ilgili dersin kazanımları atanabilir.");
        }

        var duplicatedIds = selected.GroupBy(x => x.LearningOutcomeId).Where(g => g.Count() > 1).Select(g => g.Key).ToList();
        if (duplicatedIds.Any())
        {
            throw new InvalidOperationException("Aynı kazanım bir soruya birden fazla kez atanamaz.");
        }

        var existing = await _context.QuestionOutcomes
            .Where(x => x.QuestionId == model.QuestionId)
            .ToListAsync();

        _context.QuestionOutcomes.RemoveRange(existing);

        var now = DateTime.UtcNow;
        foreach (var item in selected)
        {
            _context.QuestionOutcomes.Add(new QuestionOutcome
            {
                QuestionId = model.QuestionId,
                LearningOutcomeId = item.LearningOutcomeId,
                Weight = item.Weight,
                CreatedAt = now
            });
        }

        await _context.SaveChangesAsync();
    }

    public async Task DeleteAssignmentsByQuestionIdAsync(int questionId)
    {
        var existing = await _context.QuestionOutcomes
            .Where(x => x.QuestionId == questionId)
            .ToListAsync();

        if (existing.Count == 0)
        {
            return;
        }

        _context.QuestionOutcomes.RemoveRange(existing);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> QuestionExistsAsync(int questionId)
    {
        return await _context.Questions.AnyAsync(x => x.Id == questionId);
    }
}
