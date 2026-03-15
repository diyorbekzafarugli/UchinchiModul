using Microsoft.Extensions.DependencyInjection;
using StudentCoursePlatform.Application.Abstractions.Repositories;
using StudentCoursePlatform.Application.Interfaces;
using StudentCoursePlatform.Infrastructure.Services;

namespace StudentCoursePlatform.Infrastructure.Security.DependencyInjection;

public static class SecurityDependencyInjection
{
    public static IServiceCollection AddSecurity(this IServiceCollection services)
    {
        services.AddHttpContextAccessor();
        services.AddSingleton<IPasswordHasher, PasswordHasher>();
        services.AddSingleton<IJwtProvider, JwtProvider>();
        services.AddScoped<ICurrentUserService, CurrentUserService>();
        services.AddScoped<IBlacklistService, BlacklistService>();

        return services;
    }
}
