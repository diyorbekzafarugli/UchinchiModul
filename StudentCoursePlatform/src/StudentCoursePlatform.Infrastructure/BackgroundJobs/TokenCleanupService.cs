using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using StudentCoursePlatform.Application.Abstractions.Repositories;
using StudentCoursePlatform.Infrastructure.Persistence;

public class TokenCleanupService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;

    public TokenCleanupService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            using var scope = _serviceProvider.CreateScope();

            var repo = scope.ServiceProvider
                .GetRequiredService<IRefreshTokenRepository>();

            var dbContext = scope.ServiceProvider
                .GetRequiredService<AppDbContext>();

            await repo.DeleteOldTokensAsync(DateTime.UtcNow.AddDays(-30));

            await dbContext.BlacklistedTokens
                .Where(t => t.ExpiresAt < DateTime.UtcNow)
                .ExecuteDeleteAsync();

            await Task.Delay(TimeSpan.FromHours(24), stoppingToken);
        }
    }
}