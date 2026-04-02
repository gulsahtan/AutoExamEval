using AutoExamEval.Data;
using AutoExamEval.Entities;
using AutoExamEval.Enums;
using AutoExamEval.Services.Interfaces;
using AutoExamEval.ViewModels.Question;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace AutoExamEval.Services;

public class QuestionService : IQuestionService
{
    private readonly ApplicationDbContext _context;

    public QuestionService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<QuestionListViewModel>> GetAllAsync()
    {
        return await _context.Questions
            .AsNoTracking()
            .Include(x => x.Exam)
            .OrderBy(x => x.ExamId)
            .ThenBy(x => x.QuestionNo)
            .Select(x => new QuestionListViewModel
            {
                Id = x.Id,
                ExamId = x.ExamId,
                ExamTitle = x.Exam != null ? x.Exam.ExamTitle : "-",
                QuestionNo = x.QuestionNo,
                QuestionText = x.QuestionText,
                QuestionType = x.QuestionType,
                Score = x.Score
            })
            .ToListAsync();
    }

    public async Task<Question?> GetByIdAsync(int id)
    {
        return await _context.Questions
            .AsNoTracking()
            .Include(x => x.Exam)
            .ThenInclude(x => x!.Course)
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<List<QuestionListViewModel>> GetByExamIdAsync(int examId)
    {
        return await _context.Questions
            .AsNoTracking()
            .Include(x => x.Exam)
            .Where(x => x.ExamId == examId)
            .OrderBy(x => x.QuestionNo)
            .Select(x => new QuestionListViewModel
            {
                Id = x.Id,
                ExamId = x.ExamId,
                ExamTitle = x.Exam != null ? x.Exam.ExamTitle : "-",
                QuestionNo = x.QuestionNo,
                QuestionText = x.QuestionText,
                QuestionType = x.QuestionType,
                Score = x.Score
            })
            .ToListAsync();
    }

    public async Task<int> CreateAsync(QuestionCreateViewModel model)
    {
        await EnsureUniqueQuestionNoAsync(model.ExamId, model.QuestionNo!.Value, null);

        var now = DateTime.UtcNow;
        var question = new Question
        {
            ExamId = model.ExamId,
            QuestionNo = model.QuestionNo.Value,
            QuestionText = model.QuestionText.Trim(),
            QuestionType = model.QuestionType!.Value,
            Score = model.Score!.Value,
            OptionA = Normalize(model.OptionA),
            OptionB = Normalize(model.OptionB),
            OptionC = Normalize(model.OptionC),
            OptionD = Normalize(model.OptionD),
            OptionE = Normalize(model.OptionE),
            CorrectAnswer = Normalize(model.CorrectAnswer),
            AnswerText = Normalize(model.AnswerText),
            CreatedAt = now,
            UpdatedAt = now
        };

        NormalizeByQuestionType(question);

        _context.Questions.Add(question);
        await _context.SaveChangesAsync();

        return question.Id;
    }

    public async Task<bool> UpdateAsync(QuestionEditViewModel model)
    {
        var question = await _context.Questions.FirstOrDefaultAsync(x => x.Id == model.Id);
        if (question is null)
        {
            return false;
        }

        await EnsureUniqueQuestionNoAsync(model.ExamId, model.QuestionNo!.Value, model.Id);

        question.ExamId = model.ExamId;
        question.QuestionNo = model.QuestionNo.Value;
        question.QuestionText = model.QuestionText.Trim();
        question.QuestionType = model.QuestionType!.Value;
        question.Score = model.Score!.Value;
        question.OptionA = Normalize(model.OptionA);
        question.OptionB = Normalize(model.OptionB);
        question.OptionC = Normalize(model.OptionC);
        question.OptionD = Normalize(model.OptionD);
        question.OptionE = Normalize(model.OptionE);
        question.CorrectAnswer = Normalize(model.CorrectAnswer);
        question.AnswerText = Normalize(model.AnswerText);
        question.UpdatedAt = DateTime.UtcNow;

        NormalizeByQuestionType(question);

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var question = await _context.Questions.FirstOrDefaultAsync(x => x.Id == id);
        if (question is null)
        {
            return false;
        }

        _context.Questions.Remove(question);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ExistsAsync(int id)
    {
        return await _context.Questions.AnyAsync(x => x.Id == id);
    }

    public async Task<int> GetNextQuestionNumberAsync(int examId)
    {
        var currentMax = await _context.Questions
            .Where(x => x.ExamId == examId)
            .MaxAsync(x => (int?)x.QuestionNo);

        return (currentMax ?? 0) + 1;
    }

    public async Task<List<SelectListItem>> GetExamSelectListAsync()
    {
        return await _context.Exams
            .AsNoTracking()
            .Include(x => x.Course)
            .OrderByDescending(x => x.ExamDate)
            .Select(x => new SelectListItem
            {
                Value = x.Id.ToString(),
                Text = (x.Course != null ? x.Course.CourseCode + " - " : string.Empty) + x.ExamTitle
            })
            .ToListAsync();
    }

    public List<SelectListItem> GetQuestionTypeSelectList()
    {
        return Enum.GetValues<QuestionType>()
            .Select(x => new SelectListItem
            {
                Value = x.ToString(),
                Text = x.ToString()
            })
            .ToList();
    }

    private async Task EnsureUniqueQuestionNoAsync(int examId, int questionNo, int? excludedQuestionId)
    {
        var query = _context.Questions.Where(x => x.ExamId == examId && x.QuestionNo == questionNo);
        if (excludedQuestionId.HasValue)
        {
            query = query.Where(x => x.Id != excludedQuestionId.Value);
        }

        if (await query.AnyAsync())
        {
            throw new InvalidOperationException("Aynı sınavda bu soru numarası zaten kullanılıyor.");
        }
    }

    private static string? Normalize(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }

    private static void NormalizeByQuestionType(Question question)
    {
        switch (question.QuestionType)
        {
            case QuestionType.MultipleChoice:
                question.CorrectAnswer = question.CorrectAnswer?.ToUpperInvariant();
                break;
            case QuestionType.TrueFalse:
                question.OptionA = null;
                question.OptionB = null;
                question.OptionC = null;
                question.OptionD = null;
                question.OptionE = null;
                if (!string.IsNullOrWhiteSpace(question.CorrectAnswer))
                {
                    var normalized = question.CorrectAnswer.Trim();
                    if (normalized.Equals("T", StringComparison.OrdinalIgnoreCase)) normalized = "True";
                    if (normalized.Equals("F", StringComparison.OrdinalIgnoreCase)) normalized = "False";
                    question.CorrectAnswer = normalized;
                }
                break;
            case QuestionType.Written:
                question.OptionA = null;
                question.OptionB = null;
                question.OptionC = null;
                question.OptionD = null;
                question.OptionE = null;
                question.CorrectAnswer = null;
                break;
            case QuestionType.FillInTheBlank:
                question.OptionA = null;
                question.OptionB = null;
                question.OptionC = null;
                question.OptionD = null;
                question.OptionE = null;
                break;
        }
    }
}
