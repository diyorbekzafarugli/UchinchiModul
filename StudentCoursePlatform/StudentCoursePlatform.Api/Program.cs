using Serilog;
using StudentCoursePlatform.Api.Extensions;
using StudentCoursePlatform.Api.Middlewares;
using StudentCoursePlatform.Application.DependencyInjection;
using StudentCoursePlatform.Infrastructure.Persistence.DependencyInjection;
using StudentCoursePlatform.Infrastructure.Security.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, _, configuration) =>
    configuration
        .ReadFrom.Configuration(context.Configuration)
        .WriteTo.Console()
        .WriteTo.File("Logs/app-.log", rollingInterval: RollingInterval.Day));

builder.Services.AddControllers();
builder.Services.AddLocalization();
builder.Services.AddSwagger();
builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.AddCorsPolicy();
builder.Services.AddValidators();
builder.Services.AddApplication();
builder.Services.AddPersistence(builder.Configuration);
builder.Services.AddSecurity();
builder.Services.AddHostedService<TokenCleanupService>();

var app = builder.Build();

app.UseRequestLocalization(options =>
{
    var cultures = new[] { "en", "ru", "uz" };
    options.SetDefaultCulture("uz")
           .AddSupportedCultures(cultures)
           .AddSupportedUICultures(cultures);
});
app.UseCors();
app.UseMiddleware<GlobalExceptionMiddleware>();
app.UseMiddleware<TokenBlacklistMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();