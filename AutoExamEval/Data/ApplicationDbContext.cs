using AutoExamEval.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AutoExamEval.Data;

public class ApplicationDbContext : IdentityDbContext<AppUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<Course> Courses => Set<Course>();
    public DbSet<Exam> Exams => Set<Exam>();
    public DbSet<Question> Questions => Set<Question>();
    public DbSet<LearningOutcome> LearningOutcomes => Set<LearningOutcome>();
    public DbSet<QuestionOutcome> QuestionOutcomes => Set<QuestionOutcome>();
    public DbSet<Student> Students => Set<Student>();
    public DbSet<OpticalReadBatch> OpticalReadBatches => Set<OpticalReadBatch>();
    public DbSet<StudentAnswer> StudentAnswers => Set<StudentAnswer>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<AppUser>(entity =>
        {
            entity.Property(x => x.FullName)
                .HasMaxLength(150)
                .IsRequired();
        });

        builder.Entity<Course>(entity =>
        {
            entity.Property(x => x.CourseCode)
                .HasMaxLength(20)
                .IsRequired();

            entity.Property(x => x.CourseName)
                .HasMaxLength(200)
                .IsRequired();

            entity.Property(x => x.Term)
                .HasMaxLength(50)
                .IsRequired();

            entity.Property(x => x.AcademicYear)
                .HasMaxLength(20)
                .IsRequired();

            entity.Property(x => x.InstructorName)
                .HasMaxLength(150)
                .IsRequired();

            entity.Property(x => x.Description)
                .HasMaxLength(1000);

            entity.Property(x => x.CreatedAt)
                .IsRequired();

            entity.Property(x => x.UpdatedAt)
                .IsRequired();
        });

        builder.Entity<LearningOutcome>(entity =>
        {
            entity.Property(x => x.OutcomeCode)
                .HasMaxLength(50)
                .IsRequired();

            entity.Property(x => x.Description)
                .HasMaxLength(1000)
                .IsRequired();

            entity.Property(x => x.Weight)
                .HasColumnType("decimal(8,2)");

            entity.Property(x => x.CreatedAt)
                .IsRequired();

            entity.Property(x => x.UpdatedAt)
                .IsRequired();

            entity.HasOne(x => x.Course)
                .WithMany(x => x.LearningOutcomes)
                .HasForeignKey(x => x.CourseId)
                .OnDelete(DeleteBehavior.Restrict);
        });


        builder.Entity<Exam>(entity =>
        {
            entity.Property(x => x.ExamTitle)
                .HasMaxLength(200)
                .IsRequired();

            entity.Property(x => x.ExamType)
                .IsRequired();

            entity.Property(x => x.ExamDate)
                .IsRequired();

            entity.Property(x => x.ExamLocation)
                .HasMaxLength(200)
                .IsRequired();

            entity.Property(x => x.InstructorName)
                .HasMaxLength(150)
                .IsRequired();

            entity.Property(x => x.DurationMinutes)
                .IsRequired();

            entity.Property(x => x.TotalScore)
                .HasColumnType("decimal(8,2)")
                .IsRequired();

            entity.Property(x => x.TemplateType)
                .HasMaxLength(100);

            entity.Property(x => x.Description)
                .HasMaxLength(1000);

            entity.Property(x => x.CreatedAt)
                .IsRequired();

            entity.Property(x => x.UpdatedAt)
                .IsRequired();

            entity.HasOne(x => x.Course)
                .WithMany(x => x.Exams)
                .HasForeignKey(x => x.CourseId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        builder.Entity<Question>(entity =>
        {
            entity.Property(x => x.QuestionNo)
                .IsRequired();

            entity.Property(x => x.QuestionText)
                .HasMaxLength(4000)
                .IsRequired();

            entity.Property(x => x.QuestionType)
                .IsRequired();

            entity.Property(x => x.OptionA)
                .HasMaxLength(1000);

            entity.Property(x => x.OptionB)
                .HasMaxLength(1000);

            entity.Property(x => x.OptionC)
                .HasMaxLength(1000);

            entity.Property(x => x.OptionD)
                .HasMaxLength(1000);

            entity.Property(x => x.OptionE)
                .HasMaxLength(1000);

            entity.Property(x => x.CorrectAnswer)
                .HasMaxLength(100);

            entity.Property(x => x.Score)
                .HasColumnType("decimal(8,2)")
                .IsRequired();

            entity.Property(x => x.AnswerText)
                .HasMaxLength(2000);

            entity.Property(x => x.CreatedAt)
                .IsRequired();

            entity.Property(x => x.UpdatedAt)
                .IsRequired();

            entity.HasIndex(x => new { x.ExamId, x.QuestionNo })
                .IsUnique();

            entity.HasOne(x => x.Exam)
                .WithMany(x => x.Questions)
                .HasForeignKey(x => x.ExamId)
                .OnDelete(DeleteBehavior.Restrict);
        });



        builder.Entity<Student>(entity =>
        {
            entity.Property(x => x.StudentNumber)
                .HasMaxLength(50)
                .IsRequired();

            entity.Property(x => x.FullName)
                .HasMaxLength(150)
                .IsRequired();

            entity.Property(x => x.Department)
                .HasMaxLength(150);

            entity.Property(x => x.ClassName)
                .HasMaxLength(50);

            entity.Property(x => x.CreatedAt)
                .IsRequired();

            entity.Property(x => x.UpdatedAt)
                .IsRequired();

            entity.HasIndex(x => x.StudentNumber)
                .IsUnique();
        });

        builder.Entity<OpticalReadBatch>(entity =>
        {
            entity.Property(x => x.BatchName)
                .HasMaxLength(150)
                .IsRequired();

            entity.Property(x => x.ImportedByUserId)
                .HasMaxLength(450);

            entity.Property(x => x.FileName)
                .HasMaxLength(260);

            entity.Property(x => x.Notes)
                .HasMaxLength(1000);

            entity.Property(x => x.ImportedAt)
                .IsRequired();

            entity.HasOne(x => x.Exam)
                .WithMany(x => x.OpticalReadBatches)
                .HasForeignKey(x => x.ExamId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        builder.Entity<StudentAnswer>(entity =>
        {
            entity.Property(x => x.GivenAnswer)
                .HasMaxLength(200);

            entity.Property(x => x.RawValue)
                .HasMaxLength(500);

            entity.Property(x => x.CreatedAt)
                .IsRequired();

            entity.Property(x => x.UpdatedAt)
                .IsRequired();

            entity.HasIndex(x => new { x.ExamId, x.StudentId, x.QuestionId })
                .IsUnique();

            entity.HasOne(x => x.Exam)
                .WithMany(x => x.StudentAnswers)
                .HasForeignKey(x => x.ExamId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(x => x.Student)
                .WithMany(x => x.StudentAnswers)
                .HasForeignKey(x => x.StudentId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(x => x.Question)
                .WithMany(x => x.StudentAnswers)
                .HasForeignKey(x => x.QuestionId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(x => x.OpticalReadBatch)
                .WithMany(x => x.StudentAnswers)
                .HasForeignKey(x => x.OpticalReadBatchId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        builder.Entity<QuestionOutcome>(entity =>
        {
            entity.Property(x => x.Weight)
                .HasColumnType("decimal(8,2)");

            entity.Property(x => x.CreatedAt)
                .IsRequired();

            entity.HasIndex(x => new { x.QuestionId, x.LearningOutcomeId })
                .IsUnique();

            entity.HasOne(x => x.Question)
                .WithMany(x => x.QuestionOutcomes)
                .HasForeignKey(x => x.QuestionId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(x => x.LearningOutcome)
                .WithMany(x => x.QuestionOutcomes)
                .HasForeignKey(x => x.LearningOutcomeId)
                .OnDelete(DeleteBehavior.Restrict);
        });


    }
}
