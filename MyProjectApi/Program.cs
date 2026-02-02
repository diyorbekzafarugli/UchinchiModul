using MyProject.Interfaces;
using MyProject.Repositories;
using MyProject.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped(typeof(IRepository<>), typeof(JsonRepository<>));

builder.Services.AddScoped(typeof(IRepository<>), typeof(JsonRepository<>));
builder.Services.AddScoped<IStudentService, StudentService>();
builder.Services.AddScoped<IProductService, ProductService>();

builder.Services.AddScoped<IStudentService, StudentService>();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
app.MapControllers();

app.Run();
