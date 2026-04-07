using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using StudentCoursePlatform.Application.Interfaces;
using StudentCoursePlatform.Application.Services;
using System.Reflection;

namespace StudentCoursePlatform.Application.DependencyInjection;

public static class ApplicationServiceCollectionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IRefreshTokenService, RefreshTokenService>();
        services.AddScoped<ICourseService, CourseService>();
        services.AddScoped<ILessonService, LessonService>();
        services.AddScoped<IEnrollmentService, EnrollmentService>();
        services.AddScoped<IHomeworkService, HomeworkService>();
        services.AddScoped<IHomeworkSubmissionService, HomeworkSubmissionService>();
        services.AddScoped<IQuizService, QuizService>();


        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        return services;
    }
}