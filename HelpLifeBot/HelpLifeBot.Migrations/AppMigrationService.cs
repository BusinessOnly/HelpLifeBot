using HelpLifeBot.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace HelpLifeBot.Migrations
{
    public sealed class AppMigrationService : BackgroundService
    {
        private readonly IHostApplicationLifetime _hostApplicationLifetime;

        private readonly AppDbContext _context;
        private readonly ILogger _logger;

        public AppMigrationService(IHostApplicationLifetime hostApplicationLifetime, AppDbContext context, ILogger<AppMigrationService> logger)
        {
            _hostApplicationLifetime = hostApplicationLifetime;
            _context = context;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Starting migrations...");
            try
            {
                await InternalExecuteAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                Environment.ExitCode = -1;
            }
            finally
            {
                _hostApplicationLifetime.StopApplication();
            }
        }

        private async Task InternalExecuteAsync(CancellationToken stoppingToken)
        {
            int attempt = 0;
            do
            {
                stoppingToken.ThrowIfCancellationRequested();
                attempt++;
                try
                {
                    await _context.Database.MigrateAsync(stoppingToken);
                    _logger.LogInformation("Migrations ended.");
                    return;
                }
                catch (Exception exception)
                {
                    _logger.LogError(exception, $"Try #{attempt} connection to Database server FAILED");
                }

                await Task.Delay(attempt * 1000, stoppingToken);
            }
            while (10 < attempt);
            throw new Exception("Attempt limit exceeded.");
        }
    }
}
