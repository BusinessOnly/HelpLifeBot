using HelpLifeBot;
using HelpLifeBot.Domain;
using HelpLifeBot.Host.Middlewares;
using MentKit.ReadModels;
using Microsoft.EntityFrameworkCore;
using NLog.Extensions.Logging;
using System.Diagnostics;

var builder = WebApplication.CreateBuilder(args);

var configuration = builder.Configuration;

var postgresConnectionString = configuration.GetConnectionString("postgres");
Debug.Assert(postgresConnectionString != null);

builder.Services.AddDbContextPool<AppDbContext>(options => options.UseNpgsql(postgresConnectionString));
builder.Services.AddEntityFrameworkReadModelExecutor();

builder.Services.AddScoped<IUserActionRepository, UserActionRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();

builder.Services.AddScoped<IBLService, BLService>();
builder.Services.AddScoped<ITgService, TgService>();

builder.Logging.ClearProviders().SetMinimumLevel(LogLevel.Trace).AddNLog();

builder.Services.AddHttpClient(Consts.SunoApi, c =>
{
    c.BaseAddress = new Uri(configuration.GetSection("Application:SunoApi:BaseAddress").Value!);
    c.DefaultRequestHeaders.Add("Accept", "application/json");
    c.DefaultRequestHeaders.Add("Authorization", $"Bearer {configuration.GetSection("Application:SunoApi:ApiKey").Value!}");
    c.Timeout = TimeSpan.FromMinutes(20);
});

builder.Services.AddHttpClient(Consts.TelegramApi, c =>
{
    c.BaseAddress = new Uri($"{configuration.GetSection("Application:TelegramApi:BaseAddress").Value!}bot{configuration.GetSection("Application:TelegramApi:ApiKey").Value!}/");
    c.DefaultRequestHeaders.Add("Accept", "application/json");
    c.Timeout = TimeSpan.FromMinutes(20);
});

// Add services to the container.

builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseMiddleware<SpecificEndpointLoggingMiddleware>();

app.UseAuthorization();

app.MapControllers();

app.Run();
