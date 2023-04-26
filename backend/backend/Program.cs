using backend;
using backend.Data;
using backend.Encryption;
using backend.Encryption.Interfaces;
using backend.HelperFunctions;
using backend.HelperFunctions.Interfaces;
using backend.Repository;
using backend.Repository.Interfaces;
using backend.SseOperations;
using backend.SseOperations.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddTransient<IFileValidator, FileValidator>();

builder.Services.AddTransient<ISseOperations, SseOperations>();
builder.Services.AddTransient<IEncryption, Encryption>();
builder.Services.AddTransient<IRepository, Repository>();

builder.Services.Configure<Keys>(builder.Configuration.GetSection("Keys"));

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
app.UseCors(builder => builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
app.UseAuthorization();

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
        Path.Combine(Directory.GetCurrentDirectory(), "TempFiles")),
    RequestPath = "/TempFiles",
    OnPrepareResponse = ctx =>
    {
        ctx.Context.Response.Headers.Append("Access-Control-Allow-Origin", "*");
    }
});

app.MapControllers();

app.Run();
