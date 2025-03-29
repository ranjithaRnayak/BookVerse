using BookVerseApp.Infrastructure.Data;
using BookVerseApp.Application.Mapping;
using Microsoft.EntityFrameworkCore;         // ✅ For EF Core
using Microsoft.OpenApi.Models;              // ✅ For Swagger

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add EF Core
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add AutoMapper
builder.Services.AddAutoMapper(typeof(BookProfile).Assembly);

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();
app.MapControllers();
app.Run();
