using AutoExamEval.Data;
using AutoExamEval.Entities;
using AutoExamEval.Enums;
using AutoExamEval.Services.Interfaces;
using AutoExamEval.ViewModels.ExamTemplate;
using Microsoft.EntityFrameworkCore;

namespace AutoExamEval.Services;

public class ExamTemplateService : IExamTemplateService
{
    private readonly ApplicationDbContext _context;

    public ExamTemplateService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ExamTemplateViewModel?> GetTemplateAsync(int examId)
    {
        var exam = await _context.Exams
            .AsNoTracking()
            .Include(x => x.Course)
            .Include(x => x.Questions)
            .FirstOrDefaultAsync(x => x.Id == examId);

        if (exam is null || exam.Course is null)
        {
            return null;
        }

        var model = new ExamTemplateViewModel
        {
            ExamId = exam.Id,
            ExamTitle = exam.ExamTitle,
            CourseCode = exam.Course.CourseCode,
            CourseName = exam.Course.CourseName,
            Term = exam.Course.Term,
            AcademicYear = exam.Course.AcademicYear,
            InstructorName = exam.InstructorName,
            ExamType = exam.ExamType,
            ExamDate = exam.ExamDate,
            ExamLocation = exam.ExamLocation,
            DurationMinutes = exam.DurationMinutes,
            TotalScore = exam.TotalScore,
            Questions = exam.Questions
                .OrderBy(x => x.QuestionNo)
                .Select(MapQuestion)
                .ToList()
        };

        model.InstructionText = BuildInstructionText(model);
        return model;
    }

    public string BuildInstructionText(ExamTemplateViewModel model)
    {
        var hasMultipleChoice = model.Questions.Any(x => x.QuestionType == QuestionType.MultipleChoice);
        var hasWritten = model.Questions.Any(x => x.QuestionType == QuestionType.Written);

        var instructions = new List<string>
        {
            "Tüm soruları dikkatlice okuyunuz.",
            "Yanıtlarınızı ayrılan alanlara yazınız/işaretleyiniz.",
            "Kimlik bilgilerinizi eksiksiz doldurunuz."
        };

        if (hasMultipleChoice)
        {
            instructions.Add("Test sorularında yalnızca bir seçeneği işaretleyiniz.");
            instructions.Add("Optik işaretleme alanlarını taşırmadan doldurunuz.");
        }

        if (hasWritten)
        {
            instructions.Add("Yazılı sorularda cevaplarınızı okunaklı şekilde yazınız.");
        }

        return string.Join(" ", instructions);
    }

    private static ExamTemplateQuestionViewModel MapQuestion(Question question)
    {
        return new ExamTemplateQuestionViewModel
        {
            QuestionNo = question.QuestionNo,
            QuestionText = question.QuestionText,
            QuestionType = question.QuestionType,
            Score = question.Score,
            OptionA = question.OptionA,
            OptionB = question.OptionB,
            OptionC = question.OptionC,
            OptionD = question.OptionD,
            OptionE = question.OptionE,
            HasOptions = question.QuestionType == QuestionType.MultipleChoice,
            NeedsLongAnswerArea = question.QuestionType == QuestionType.Written,
            NeedsShortAnswerArea = question.QuestionType == QuestionType.FillInTheBlank,
            ShowTrueFalseBoxes = question.QuestionType == QuestionType.TrueFalse
        };
    }
}
