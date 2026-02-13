using Microsoft.OpenApi;
using MySocialMedia.Api.Repositories;
using MySocialMedia.Api.Services;

namespace MySocialMedia.Api;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddSingleton<IUserRepository, UserRepository>();
        builder.Services.AddSingleton<IPostRepository, PostRepository>();

        builder.Services.AddScoped<IUserService, UserService>();
        builder.Services.AddScoped<IPostService, PostService>();
        builder.Services.AddScoped<IAuthorizeService, AuthorizeService>();

        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();


        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.MapControllers();

        app.Run();
    }
}