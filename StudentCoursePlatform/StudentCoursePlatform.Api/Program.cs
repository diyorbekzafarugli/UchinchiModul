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

builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");

builder.Services.AddControllers()
    .AddDataAnnotationsLocalization(options =>
    {
        options.DataAnnotationLocalizerProvider = (type, factory) =>
            factory.Create(typeof(StudentCoursePlatform.Application.Resources.SharedResource));
    });

builder.Services.AddSwagger();
builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.AddCorsPolicy();
builder.Services.AddApplication();
builder.Services.AddPersistence(builder.Configuration);
builder.Services.AddSecurity();
builder.Services.AddHostedService<TokenCleanupService>();

var app = builder.Build();

var supportedCultures = new[] { "uz", "en", "ru" };
var localizationOptions = new RequestLocalizationOptions()
    .SetDefaultCulture(supportedCultures[0])
    .AddSupportedCultures(supportedCultures)
    .AddSupportedUICultures(supportedCultures);

app.UseRequestLocalization(localizationOptions);

app.UseCors();
app.UseStaticFiles();
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