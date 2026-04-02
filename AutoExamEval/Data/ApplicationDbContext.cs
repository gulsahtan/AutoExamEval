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
