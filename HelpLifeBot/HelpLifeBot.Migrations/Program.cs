using System.Reflection;
using HelpLifeBot.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace HelpLifeBot.Migrations
{
    public class Program
    {
        static async Task Main(string[] args) =>
            await CreateHostBuilder(args)
                .UseConsoleLifetime()
                .Build()
                .RunAsync();

        static IHostBuilder CreateHostBuilder(string[] args) =>
            new HostBuilder()
                .ConfigureAppConfiguration((hostContext, configApp) =>
                {
                    configApp.SetBasePath(Directory.GetCurrentDirectory());
                    configApp.AddJsonFile("appsettings.json", optional: true);
                    configApp.AddJsonFile(
                        $"appsettings.{hostContext.HostingEnvironment.EnvironmentName}.json",
                        optional: true);
                    configApp.AddEnvironmentVariables();
                })
                .UseSerilog((whb, ctx) =>
                    ctx.ReadFrom.Configuration(whb.Configuration)
                        .Enrich.FromLogContext()
                )
                .ConfigureServices((hostContext, services) =>
                {
                    var connectionString = hostContext.Configuration.GetConnectionString("postgres");

                    services.AddDbContext<AppDbContext>(options => options.UseNpgsql(connectionString,
                        b => b.MigrationsAssembly(Assembly.GetExecutingAssembly().FullName)));
                    services.AddHostedService<AppMigrationService>();
                });
    }
}
