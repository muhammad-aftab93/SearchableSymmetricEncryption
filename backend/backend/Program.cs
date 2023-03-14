using backend.Data;
using backend.Encryption;
using backend.Encryption.Interfaces;
using backend.Repository;
using backend.Repository.Interfaces;
using backend.SseOperations;
using backend.SseOperations.Interfaces;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddTransient<IRepository, Repository>();
builder.Services.AddTransient<IEncryption, Encryption>();
builder.Services.AddTransient<ISseOperations, SseOperations>();

builder.Services.AddDbContext<TDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
}, ServiceLifetime.Transient);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
