using System.Globalization;
using AutoExamEval.Data;
using AutoExamEval.Entities;
using AutoExamEval.Enums;
using AutoExamEval.Services.Interfaces;
using AutoExamEval.ViewModels.AnswerImport;
using Microsoft.EntityFrameworkCore;

namespace AutoExamEval.Services;

public class AnswerImportService : IAnswerImportService
{
    private readonly ApplicationDbContext _context;

    public AnswerImportService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<AnswerImportResultViewModel> ImportFromCsvAsync(AnswerImportCsvViewModel model, string? userId)
    {
        if (model.CsvFile is null || model.CsvFile.Length == 0)
        {
            throw new InvalidOperationException("CSV dosyası boş olamaz.");
        }

        var exam = await _context.Exams
            .AsNoTracking()
            .Include(x => x.Questions)
            .FirstOrDefaultAsync(x => x.Id == model.ExamId);

        if (exam is null)
        {
            throw new InvalidOperationException("Sınav bulunamadı.");
        }

        var lines = await ParseCsvAsync(model.CsvFile);
        if (lines.Count < 2)
        {
            throw new InvalidOperationException("CSV dosyasında veri bulunamadı.");
        }

        var header = SplitCsvLine(lines[0]);
        ValidateHeader(header);

        var questionMap = MapColumnsToQuestions(header, exam.Questions);

        var batch = new OpticalReadBatch
        {
            ExamId = exam.Id,
            BatchName = model.BatchName.Trim(),
            ImportedAt = DateTime.UtcNow,
            ImportedByUserId = userId,
            SourceType = ImportSourceType.Csv,
            FileName = model.CsvFile.FileName,
            Notes = model.Notes,
            TotalRecordCount = lines.Count - 1
        };

        _context.OpticalReadBatches.Add(batch);
        await _context.SaveChangesAsync();

        var result = new AnswerImportResultViewModel
        {
            ExamId = exam.Id,
            ExamTitle = exam.ExamTitle,
            BatchId = batch.Id,
            BatchName = batch.BatchName,
            TotalRecordCount = lines.Count - 1
        };

        for (var i = 1; i < lines.Count; i++)
        {
            var columns = SplitCsvLine(lines[i]);
            if (columns.Length < 2)
            {
                result.FailedRecordCount++;
                result.Errors.Add($"Satır {i + 1}: StudentNumber/FullName eksik.");
                continue;
            }

            var studentNumber = columns[0].Trim();
            var fullName = columns[1].Trim();

            if (string.IsNullOrWhiteSpace(studentNumber) || string.IsNullOrWhiteSpace(fullName))
            {
                result.FailedRecordCount++;
                result.Errors.Add($"Satır {i + 1}: StudentNumber veya FullName boş.");
                continue;
            }

            try
            {
                var student = await GetOrCreateStudentAsync(studentNumber, fullName);
                var importedCount = 0;

                foreach (var map in questionMap)
                {
                    if (map.ColumnIndex >= columns.Length)
                    {
                        continue;
                    }

                    var raw = columns[map.ColumnIndex];
                    var normalized = NormalizeAnswer(raw);

                    var existing = await _context.StudentAnswers
                        .FirstOrDefaultAsync(x => x.ExamId == exam.Id && x.StudentId == student.Id && x.QuestionId == map.QuestionId);

                    if (existing is null)
                    {
                        _context.StudentAnswers.Add(new StudentAnswer
                        {
                            ExamId = exam.Id,
                            StudentId = student.Id,
                            QuestionId = map.QuestionId,
                            OpticalReadBatchId = batch.Id,
                            GivenAnswer = normalized,
                            IsImported = true,
                            ImportedAt = DateTime.UtcNow,
                            RawValue = raw,
                            CreatedAt = DateTime.UtcNow,
                            UpdatedAt = DateTime.UtcNow
                        });
                    }
                    else
                    {
                        existing.GivenAnswer = normalized;
                        existing.RawValue = raw;
                        existing.IsImported = true;
                        existing.ImportedAt = DateTime.UtcNow;
                        existing.OpticalReadBatchId = batch.Id;
                        existing.UpdatedAt = DateTime.UtcNow;
                    }

                    importedCount++;
                }

                await _context.SaveChangesAsync();

                result.SuccessfulRecordCount++;
                result.ImportedRowsSummary.Add(new ImportedStudentAnswerViewModel
                {
                    StudentNumber = student.StudentNumber,
                    FullName = student.FullName,
                    ImportedAnswerCount = importedCount,
                    StatusMessage = "Başarılı"
                });
            }
            catch (Exception ex)
            {
                result.FailedRecordCount++;
                result.Errors.Add($"Satır {i + 1}: {ex.Message}");
            }
        }

        batch.SuccessfulRecordCount = result.SuccessfulRecordCount;
        batch.FailedRecordCount = result.FailedRecordCount;
        await _context.SaveChangesAsync();

        return result;
    }

    public async Task<ManualAnswerEntryViewModel?> GetManualEntryViewModelAsync(int examId)
    {
        var exam = await _context.Exams
            .AsNoTracking()
            .Include(x => x.Questions)
            .FirstOrDefaultAsync(x => x.Id == examId);

        if (exam is null)
        {
            return null;
        }

        return new ManualAnswerEntryViewModel
        {
            ExamId = exam.Id,
            ExamTitle = exam.ExamTitle,
            Answers = exam.Questions
                .OrderBy(x => x.QuestionNo)
                .Select(x => new ManualAnswerItemViewModel
                {
                    QuestionId = x.Id,
                    QuestionNo = x.QuestionNo,
                    QuestionText = x.QuestionText,
                    QuestionType = x.QuestionType
                })
                .ToList()
        };
    }

    public async Task<AnswerImportResultViewModel> SaveManualAnswersAsync(ManualAnswerEntryViewModel model, string? userId)
    {
        var exam = await _context.Exams
            .AsNoTracking()
            .Include(x => x.Questions)
            .FirstOrDefaultAsync(x => x.Id == model.ExamId);

        if (exam is null)
        {
            throw new InvalidOperationException("Sınav bulunamadı.");
        }

        var batch = new OpticalReadBatch
        {
            ExamId = exam.Id,
            BatchName = $"Manual-{DateTime.UtcNow:yyyyMMddHHmmss}",
            ImportedAt = DateTime.UtcNow,
            ImportedByUserId = userId,
            SourceType = ImportSourceType.Manual,
            FileName = null,
            Notes = "Manual answer entry",
            TotalRecordCount = 1,
            SuccessfulRecordCount = 1,
            FailedRecordCount = 0
        };

        _context.OpticalReadBatches.Add(batch);
        await _context.SaveChangesAsync();

        var student = await GetOrCreateStudentAsync(model.StudentNumber.Trim(), model.FullName.Trim());

        var importedCount = 0;
        foreach (var item in model.Answers)
        {
            var existing = await _context.StudentAnswers
                .FirstOrDefaultAsync(x => x.ExamId == exam.Id && x.StudentId == student.Id && x.QuestionId == item.QuestionId);

            var normalized = NormalizeAnswer(item.GivenAnswer);

            if (existing is null)
            {
                _context.StudentAnswers.Add(new StudentAnswer
                {
                    ExamId = exam.Id,
                    StudentId = student.Id,
                    QuestionId = item.QuestionId,
                    OpticalReadBatchId = batch.Id,
                    GivenAnswer = normalized,
                    IsImported = false,
                    ImportedAt = DateTime.UtcNow,
                    RawValue = item.GivenAnswer,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                });
            }
            else
            {
                existing.GivenAnswer = normalized;
                existing.RawValue = item.GivenAnswer;
                existing.OpticalReadBatchId = batch.Id;
                existing.UpdatedAt = DateTime.UtcNow;
            }

            importedCount++;
        }

        await _context.SaveChangesAsync();

        return new AnswerImportResultViewModel
        {
            ExamId = exam.Id,
            ExamTitle = exam.ExamTitle,
            BatchId = batch.Id,
            BatchName = batch.BatchName,
            TotalRecordCount = 1,
            SuccessfulRecordCount = 1,
            FailedRecordCount = 0,
            ImportedRowsSummary = new List<ImportedStudentAnswerViewModel>
            {
                new()
                {
                    StudentNumber = student.StudentNumber,
                    FullName = student.FullName,
                    ImportedAnswerCount = importedCount,
                    StatusMessage = "Manual entry saved"
                }
            }
        };
    }

    public async Task<AnswerImportResultViewModel?> GetBatchDetailsAsync(int batchId)
    {
        var batch = await _context.OpticalReadBatches
            .AsNoTracking()
            .Include(x => x.Exam)
            .FirstOrDefaultAsync(x => x.Id == batchId);

        if (batch is null || batch.Exam is null)
        {
            return null;
        }

        var rows = await _context.StudentAnswers
            .AsNoTracking()
            .Include(x => x.Student)
            .Where(x => x.OpticalReadBatchId == batchId)
            .GroupBy(x => new { x.StudentId, x.Student!.StudentNumber, x.Student.FullName })
            .Select(g => new ImportedStudentAnswerViewModel
            {
                StudentNumber = g.Key.StudentNumber,
                FullName = g.Key.FullName,
                ImportedAnswerCount = g.Count(),
                StatusMessage = "Imported"
            })
            .ToListAsync();

        return new AnswerImportResultViewModel
        {
            ExamId = batch.ExamId,
            ExamTitle = batch.Exam.ExamTitle,
            BatchId = batch.Id,
            BatchName = batch.BatchName,
            TotalRecordCount = batch.TotalRecordCount,
            SuccessfulRecordCount = batch.SuccessfulRecordCount,
            FailedRecordCount = batch.FailedRecordCount,
            ImportedRowsSummary = rows
        };
    }

    public async Task<List<OpticalReadBatch>> GetExamBatchesAsync(int examId)
    {
        return await _context.OpticalReadBatches
            .AsNoTracking()
            .Where(x => x.ExamId == examId)
            .OrderByDescending(x => x.ImportedAt)
            .ToListAsync();
    }

    private static void ValidateHeader(string[] header)
    {
        if (header.Length < 3)
        {
            throw new InvalidOperationException("CSV header en az StudentNumber, FullName ve bir soru kolonu içermelidir.");
        }

        if (!header[0].Trim().Equals("StudentNumber", StringComparison.OrdinalIgnoreCase)
            || !header[1].Trim().Equals("FullName", StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException("CSV header StudentNumber,FullName ile başlamalıdır.");
        }
    }

    private static List<(int ColumnIndex, int QuestionNo, int QuestionId)> MapColumnsToQuestions(string[] header, ICollection<Question> questions)
    {
        var questionNoMap = questions.ToDictionary(x => x.QuestionNo, x => x.Id);
        var result = new List<(int ColumnIndex, int QuestionNo, int QuestionId)>();

        for (var i = 2; i < header.Length; i++)
        {
            var raw = header[i].Trim();
            if (!raw.StartsWith("Q", StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException($"Geçersiz soru kolonu: {raw}. Q1,Q2... formatı kullanılmalıdır.");
            }

            var noPart = raw[1..];
            if (!int.TryParse(noPart, NumberStyles.Integer, CultureInfo.InvariantCulture, out var questionNo))
            {
                throw new InvalidOperationException($"Soru kolonu çözümlenemedi: {raw}");
            }

            if (!questionNoMap.TryGetValue(questionNo, out var questionId))
            {
                throw new InvalidOperationException($"CSV kolonu {raw}, sınav sorularında bulunamadı.");
            }

            result.Add((i, questionNo, questionId));
        }

        return result;
    }

    private static string NormalizeAnswer(string? value)
    {
        var normalized = (value ?? string.Empty).Trim();
        if (string.IsNullOrWhiteSpace(normalized))
        {
            return string.Empty;
        }

        normalized = normalized.ToUpperInvariant();
        return normalized switch
        {
            "T" => "TRUE",
            "F" => "FALSE",
            "D" => "TRUE",
            "Y" => "FALSE",
            _ => normalized
        };
    }

    private static async Task<List<string>> ParseCsvAsync(Microsoft.AspNetCore.Http.IFormFile file)
    {
        var lines = new List<string>();
        using var reader = new StreamReader(file.OpenReadStream());
        while (!reader.EndOfStream)
        {
            var line = await reader.ReadLineAsync();
            if (string.IsNullOrWhiteSpace(line))
            {
                continue;
            }
            lines.Add(line);
        }
        return lines;
    }

    private async Task<Student> GetOrCreateStudentAsync(string studentNumber, string fullName)
    {
        var existing = await _context.Students.FirstOrDefaultAsync(x => x.StudentNumber == studentNumber);
        if (existing is not null)
        {
            if (!existing.FullName.Equals(fullName, StringComparison.Ordinal))
            {
                existing.FullName = fullName;
                existing.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }
            return existing;
        }

        var student = new Student
        {
            StudentNumber = studentNumber,
            FullName = fullName,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.Students.Add(student);
        await _context.SaveChangesAsync();
        return student;
    }

    private static string[] SplitCsvLine(string line)
    {
        return line.Split(',', StringSplitOptions.None);
    }
}
