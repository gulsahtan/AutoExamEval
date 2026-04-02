using AutoExamEval.Data;
using AutoExamEval.Entities;
using AutoExamEval.Enums;
using AutoExamEval.Services.Interfaces;
using AutoExamEval.ViewModels.Analysis;
using Microsoft.EntityFrameworkCore;

namespace AutoExamEval.Services;

public class ExamAnalysisService : IExamAnalysisService
{
    private readonly ApplicationDbContext _context;

    public ExamAnalysisService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ExamAnalysisSummaryViewModel?> GetExamSummaryAsync(int examId)
    {
        var full = await GetFullAnalysisAsync(examId);
        return full?.Summary;
    }

    public async Task<List<StudentPerformanceViewModel>?> GetStudentPerformancesAsync(int examId)
    {
        var full = await GetFullAnalysisAsync(examId);
        return full?.StudentPerformances;
    }

    public async Task<List<QuestionPerformanceViewModel>?> GetQuestionPerformancesAsync(int examId)
    {
        var full = await GetFullAnalysisAsync(examId);
        return full?.QuestionPerformances;
    }

    public async Task<List<LearningOutcomePerformanceViewModel>?> GetLearningOutcomePerformancesAsync(int examId)
    {
        var full = await GetFullAnalysisAsync(examId);
        return full?.LearningOutcomePerformances;
    }

    public async Task<ExamFullAnalysisViewModel?> GetFullAnalysisAsync(int examId)
    {
        var exam = await _context.Exams
            .AsNoTracking()
            .Include(x => x.Course)
            .Include(x => x.Questions)
            .FirstOrDefaultAsync(x => x.Id == examId);

        if (exam is null)
        {
            return null;
        }

        var questions = exam.Questions.OrderBy(x => x.QuestionNo).ToList();
        var questionIds = questions.Select(x => x.Id).ToHashSet();

        var answers = await _context.StudentAnswers
            .AsNoTracking()
            .Include(x => x.Student)
            .Where(x => x.ExamId == examId && questionIds.Contains(x.QuestionId))
            .ToListAsync();

        var students = answers
            .Where(x => x.Student is not null)
            .GroupBy(x => x.StudentId)
            .Select(g => g.First().Student!)
            .OrderBy(x => x.StudentNumber)
            .ToList();

        var answersByQuestionId = answers
            .GroupBy(x => x.QuestionId)
            .ToDictionary(g => g.Key, g => g.ToList());

        var answersByStudentId = answers
            .GroupBy(x => x.StudentId)
            .ToDictionary(g => g.Key, g => g.ToDictionary(x => x.QuestionId, x => x));

        var questionPerformances = questions
            .Select(question => BuildQuestionPerformance(question, answersByQuestionId.GetValueOrDefault(question.Id) ?? new List<StudentAnswer>()))
            .ToList();

        var studentPerformances = students
            .Select(student => BuildStudentPerformance(student, questions, answersByStudentId.GetValueOrDefault(student.Id) ?? new Dictionary<int, StudentAnswer>()))
            .ToList();

        var summary = BuildSummary(exam, students.Count, answers.Count, questions.Count, studentPerformances);

        var outcomePerformances = await BuildLearningOutcomePerformancesAsync(exam, questionPerformances, answersByQuestionId);

        return new ExamFullAnalysisViewModel
        {
            Summary = summary,
            StudentPerformances = studentPerformances,
            QuestionPerformances = questionPerformances,
            LearningOutcomePerformances = outcomePerformances
        };
    }

    private static ExamAnalysisSummaryViewModel BuildSummary(Exam exam, int totalStudents, int totalAnswers, int totalQuestions, List<StudentPerformanceViewModel> students)
    {
        var averageScore = students.Count == 0 ? 0 : students.Average(x => x.TotalScore);
        var highestScore = students.Count == 0 ? 0 : students.Max(x => x.TotalScore);
        var lowestScore = students.Count == 0 ? 0 : students.Min(x => x.TotalScore);
        var averageSuccessRate = students.Count == 0 ? 0 : students.Average(x => x.SuccessRate);

        return new ExamAnalysisSummaryViewModel
        {
            ExamId = exam.Id,
            ExamTitle = exam.ExamTitle,
            CourseName = exam.Course is null ? "-" : $"{exam.Course.CourseCode} - {exam.Course.CourseName}",
            TotalStudents = totalStudents,
            TotalQuestions = totalQuestions,
            TotalAnswers = totalAnswers,
            AverageScore = decimal.Round(averageScore, 2),
            HighestScore = decimal.Round(highestScore, 2),
            LowestScore = decimal.Round(lowestScore, 2),
            AverageSuccessRate = decimal.Round(averageSuccessRate, 2)
        };
    }

    private static StudentPerformanceViewModel BuildStudentPerformance(Student student, List<Question> questions, Dictionary<int, StudentAnswer> studentAnswers)
    {
        var model = new StudentPerformanceViewModel
        {
            StudentId = student.Id,
            StudentNumber = student.StudentNumber,
            FullName = student.FullName,
            NeedsManualEvaluation = questions.Any(x => x.QuestionType == QuestionType.Written)
        };

        decimal earnedScore = 0;
        decimal gradableTotalScore = 0;

        foreach (var question in questions)
        {
            studentAnswers.TryGetValue(question.Id, out var answer);
            var normalizedGivenAnswer = NormalizeAnswer(answer?.GivenAnswer);

            if (string.IsNullOrWhiteSpace(normalizedGivenAnswer))
            {
                model.BlankCount++;
                continue;
            }

            if (!IsAutomaticallyGradable(question.QuestionType))
            {
                continue;
            }

            gradableTotalScore += question.Score;

            if (IsCorrectAnswer(question, answer?.GivenAnswer))
            {
                model.CorrectCount++;
                earnedScore += question.Score;
            }
            else
            {
                model.WrongCount++;
            }
        }

        model.TotalScore = CalculateStudentScore(earnedScore);
        model.SuccessRate = gradableTotalScore <= 0 ? 0 : decimal.Round(earnedScore / gradableTotalScore * 100m, 2);

        return model;
    }

    private static QuestionPerformanceViewModel BuildQuestionPerformance(Question question, List<StudentAnswer> questionAnswers)
    {
        var model = new QuestionPerformanceViewModel
        {
            QuestionId = question.Id,
            QuestionNo = question.QuestionNo,
            QuestionType = question.QuestionType,
            QuestionText = question.QuestionText,
            Score = question.Score
        };

        foreach (var answer in questionAnswers)
        {
            var normalizedGiven = NormalizeAnswer(answer.GivenAnswer);
            if (string.IsNullOrWhiteSpace(normalizedGiven))
            {
                model.BlankCount++;
                continue;
            }

            model.TotalResponses++;

            if (!IsAutomaticallyGradable(question.QuestionType))
            {
                continue;
            }

            if (IsCorrectAnswer(question, answer.GivenAnswer))
            {
                model.CorrectCount++;
            }
            else
            {
                model.WrongCount++;
            }
        }

        model.SuccessRate = CalculateQuestionSuccessRate(model.CorrectCount, model.CorrectCount + model.WrongCount);
        model.DifficultyIndex = model.TotalResponses <= 0
            ? 0
            : decimal.Round(model.CorrectCount / (decimal)model.TotalResponses, 4);

        return model;
    }

    private async Task<List<LearningOutcomePerformanceViewModel>> BuildLearningOutcomePerformancesAsync(
        Exam exam,
        List<QuestionPerformanceViewModel> questionPerformances,
        IReadOnlyDictionary<int, List<StudentAnswer>> answersByQuestionId)
    {
        var questionIds = exam.Questions.Select(x => x.Id).ToList();
        if (!questionIds.Any())
        {
            return new List<LearningOutcomePerformanceViewModel>();
        }

        var questionOutcomes = await _context.QuestionOutcomes
            .AsNoTracking()
            .Include(x => x.LearningOutcome)
            .Where(x => questionIds.Contains(x.QuestionId))
            .ToListAsync();

        var byOutcome = questionOutcomes
            .Where(x => x.LearningOutcome is not null)
            .GroupBy(x => x.LearningOutcomeId)
            .ToList();

        var questionPerformanceMap = questionPerformances.ToDictionary(x => x.QuestionId, x => x);

        var results = new List<LearningOutcomePerformanceViewModel>();

        foreach (var group in byOutcome)
        {
            var sample = group.First();
            var outcome = sample.LearningOutcome;
            if (outcome is null)
            {
                continue;
            }

            var relatedQuestionIds = group.Select(x => x.QuestionId).Distinct().ToList();
            var relatedPerformances = relatedQuestionIds
                .Where(questionPerformanceMap.ContainsKey)
                .Select(id => questionPerformanceMap[id])
                .ToList();

            var interactionCount = relatedQuestionIds
                .Select(id => answersByQuestionId.GetValueOrDefault(id) ?? new List<StudentAnswer>())
                .SelectMany(x => x)
                .Count(x => !string.IsNullOrWhiteSpace(NormalizeAnswer(x.GivenAnswer)));

            var averageSuccessRate = relatedPerformances.Count == 0
                ? 0
                : decimal.Round(relatedPerformances.Average(x => x.SuccessRate), 2);

            decimal weightedContribution = 0;
            decimal totalWeight = 0;

            foreach (var item in group)
            {
                var weight = item.Weight ?? 1m;
                if (weight <= 0)
                {
                    continue;
                }

                if (!questionPerformanceMap.TryGetValue(item.QuestionId, out var perf))
                {
                    continue;
                }

                weightedContribution += (perf.SuccessRate / 100m) * perf.Score * weight;
                totalWeight += weight;
            }

            var averageContributionScore = totalWeight <= 0
                ? 0
                : decimal.Round(weightedContribution / totalWeight, 2);

            results.Add(new LearningOutcomePerformanceViewModel
            {
                LearningOutcomeId = outcome.Id,
                OutcomeCode = outcome.OutcomeCode,
                OutcomeDescription = outcome.Description,
                RelatedQuestionCount = relatedQuestionIds.Count,
                StudentInteractionCount = interactionCount,
                SuccessRate = averageSuccessRate,
                AverageContributionScore = averageContributionScore
            });
        }

        return results.OrderBy(x => x.OutcomeCode).ToList();
    }

    private static string NormalizeAnswer(string? answer)
    {
        return string.IsNullOrWhiteSpace(answer)
            ? string.Empty
            : answer.Trim().ToUpperInvariant();
    }

    private static bool IsAutomaticallyGradable(QuestionType questionType)
    {
        return questionType is QuestionType.MultipleChoice
            or QuestionType.TrueFalse
            or QuestionType.FillInTheBlank;
    }

    private static bool IsCorrectAnswer(Question question, string? givenAnswer)
    {
        if (!IsAutomaticallyGradable(question.QuestionType))
        {
            return false;
        }

        var normalizedCorrect = NormalizeAnswer(question.CorrectAnswer);
        var normalizedGiven = NormalizeAnswer(givenAnswer);

        if (string.IsNullOrWhiteSpace(normalizedCorrect) || string.IsNullOrWhiteSpace(normalizedGiven))
        {
            return false;
        }

        return normalizedCorrect.Equals(normalizedGiven, StringComparison.Ordinal);
    }

    private static decimal CalculateStudentScore(decimal earnedScore)
    {
        return decimal.Round(earnedScore, 2);
    }

    private static decimal CalculateQuestionSuccessRate(int correctCount, int consideredCount)
    {
        if (consideredCount <= 0)
        {
            return 0;
        }

        return decimal.Round(correctCount / (decimal)consideredCount * 100m, 2);
    }
}
