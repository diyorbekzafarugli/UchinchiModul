using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json.Serialization;
using Order.Api.Common;
using Order.Api.Entities;
using Order.Api.Repositories;
using Order.Api.Services;

var builder = WebApplication.CreateBuilder(args);

// DI
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IRepository<Order.Api.Entities.Order>, OrderRepository>();


// Controllers + Enum string ("Paid") bo‘lib kelsin/ketlsin
builder.Services.AddControllers()
    .AddJsonOptions(opt =>
    {
        opt.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

// Validation errorlarni ApiResponse formatga o‘tkazish
builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.InvalidModelStateResponseFactory = context =>
    {
        var errors = context.ModelState
            .Where(x => x.Value?.Errors.Count > 0)
            .SelectMany(x => x.Value!.Errors)
            .Select(e => string.IsNullOrWhiteSpace(e.ErrorMessage) ? "Invalid value" : e.ErrorMessage)
            .ToList();

        var response = ApiResponses.Fail<object>("Validation error", errors, context.HttpContext);
        return new BadRequestObjectResult(response);
    };
});

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Global exception handler (eng yuqorida bo‘lgani yaxshi)
app.UseExceptionHandler(errApp =>
{
    errApp.Run(async ctx =>
    {
        ctx.Response.StatusCode = 500;
        ctx.Response.ContentType = "application/json";

        var resp = ApiResponses.Fail<object>(
            "Server error",
            new[] { "Kutilmagan xatolik yuz berdi" },
            ctx);

        await ctx.Response.WriteAsJsonAsync(resp);
    });
});

// Swagger dev’da
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// app.UseAuthorization(); // auth ishlatmasang olib tashlasa ham bo‘ladi

app.MapControllers();

app.Run();
