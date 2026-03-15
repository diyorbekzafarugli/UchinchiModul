using Microsoft.Extensions.DependencyInjection;
using StudentCoursePlatform.Application.Interfaces;
using StudentCoursePlatform.Application.Services;

namespace StudentCoursePlatform.Application.DependencyInjection;

public static class ApplicationServiceCollectionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IRefreshTokenService, RefreshTokenService>();

        return services;
    }
}
