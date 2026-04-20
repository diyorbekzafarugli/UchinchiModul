using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StudentCoursePlatform.Application.Abstractions.Repositories;
using StudentCoursePlatform.Application.Interfaces;
using StudentCoursePlatform.Infrastructure.Repositories;
using StudentCoursePlatform.Infrastructure.Services;

namespace StudentCoursePlatform.Infrastructure.Persistence.DependencyInjection;

public static class PersistenceDependencyInjection
{
    public static IServiceCollection AddPersistence(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options =>
        {
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"));
        });

        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<ICourseRepository, CourseRepository>();
        services.AddScoped<ILessonRepository, LessonRepository>();
        services.AddScoped<IEnrolmentRepository, EnrolmentRepository>();
        services.AddScoped<IQuestionRepository, QuestionRepository>();
        services.AddScoped<IHomeworkRepository, HomeworkRepository>();
        services.AddScoped<IHomeworkSubmissionRepository, HomeworkSubmissionRepository>();
        services.AddScoped<IQuizRepository, QuizRepository>();
        services.AddScoped<IAnswerOptionRepository, AnswerOptionRepository>();
        services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
        //services.AddScoped<IFileService, FileService>();
        services.AddScoped<IFileService, CloudinaryFileService>();

        return services;
    }
}
