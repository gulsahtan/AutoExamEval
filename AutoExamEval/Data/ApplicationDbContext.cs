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
    }
}
