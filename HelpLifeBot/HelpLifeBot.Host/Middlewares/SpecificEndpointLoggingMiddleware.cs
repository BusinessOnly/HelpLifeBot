using NLog;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;

namespace HelpLifeBot.Host.Middlewares
{
    public class SpecificEndpointLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly JsonSerializerOptions _jsonOptions;
        // private static readonly Logger _loggerFile = LogManager.GetLogger("HelpLifeBot.Host.Controllers.Api.TelegramController.WebhookAsync.File");
        private static readonly Logger _logger = LogManager.GetLogger("HelpLifeBot.Host.Controllers.Api.TelegramController.WebhookAsync");

        public SpecificEndpointLoggingMiddleware(RequestDelegate next)
        {
            _next = next;
            _jsonOptions = new JsonSerializerOptions
            {
                Encoder = JavaScriptEncoder.Create(UnicodeRanges.BasicLatin, UnicodeRanges.Cyrillic)
            };
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Проверяем, это наш целевой метод?
            if (IsTargetEndpoint(context))
            {
                await LogRequestAsync(context);
            }

            await _next(context);
        }

        private bool IsTargetEndpoint(HttpContext context)
        {
            return context.Request.Method == "POST" &&
                   context.Request.Path.StartsWithSegments("/api/telegram/webhook");
        }

        private async Task LogRequestAsync(HttpContext context)
        {
            context.Request.EnableBuffering();

            using var reader = new StreamReader(context.Request.Body, leaveOpen: true);
            var requestBody = (await reader.ReadToEndAsync()).Replace("\n", "");
            context.Request.Body.Position = 0;

            var logData = new
            {
                Endpoint = $"{context.Request.Method} {context.Request.Path}",
                Headers = context.Request.Headers
                    .Where(h => h.Key != "Authorization")
                    .ToDictionary(h => h.Key, h => h.Value.ToString()),
                IP = $"{context.Connection.RemoteIpAddress}"
            };

            var logEvent = new LogEventInfo(NLog.LogLevel.Info, "", "Request to TelegramWebhook endpoint");
            logEvent.Properties["LogData"] = JsonSerializer.Serialize(logData, _jsonOptions);
            logEvent.Properties["RequestBody"] = requestBody;

            //_loggerFile.Log(logEvent);
            _logger.Log(logEvent);
        }
    }
}
