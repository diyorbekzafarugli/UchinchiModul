using Microsoft.EntityFrameworkCore;
using StudentCoursePlatform.Domain.Entities;

namespace StudentCoursePlatform.Infrastructure.Persistence;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<Course> Courses => Set<Course>();
    public DbSet<Lesson> Lessons => Set<Lesson>();
    public DbSet<Enrollment> Enrollments => Set<Enrollment>();
    public DbSet<Homework> Homeworks => Set<Homework>();
    public DbSet<HomeworkSubmission> HomeworkSubmissions => Set<HomeworkSubmission>();
    public DbSet<Quiz> Quizzes => Set<Quiz>();
    public DbSet<Question> Questions => Set<Question>();
    public DbSet<AnswerOption> AnswerOptions => Set<AnswerOption>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
    public DbSet<BlacklistedToken> BlacklistedTokens => Set<BlacklistedToken>();
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>().ToTable("Users");
        modelBuilder.Entity<Course>();
        modelBuilder.Entity<Lesson>();
        modelBuilder.Entity<Enrollment>();
        modelBuilder.Entity<Homework>();
        modelBuilder.Entity<HomeworkSubmission>();
        modelBuilder.Entity<Quiz>();
        modelBuilder.Entity<Question>();
        modelBuilder.Entity<AnswerOption>();
        modelBuilder.Entity<BlacklistedToken>()
            .HasKey(t => t.Token);

        modelBuilder.Entity<RefreshToken>()
            .HasOne(rt => rt.User)
            .WithMany(u => u.RefreshTokens)
            .HasForeignKey(rt => rt.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }

}
