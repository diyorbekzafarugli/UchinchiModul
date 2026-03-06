using Microsoft.EntityFrameworkCore;
using StudentCoursePlatform.Infrastructure.Persistence;
using StudentCoursePlatform.Infrastructure.Persistence.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();


builder.Services.AddPersistence(builder.Configuration);
var app = builder.Build();


builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection")));

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();



app.MapGet("/weatherforecast", () =>
{
})
.WithName("GetWeatherForecast");

app.Run();
