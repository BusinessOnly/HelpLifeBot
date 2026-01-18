using HelpLifeBot.Bindings.Telegram;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace HelpLifeBot.Host.Controllers.Api
{
    [ApiController]
    [Consumes("application/json")]
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class TelegramController : ControllerBase
    {
        private readonly ITgService _tgService;
        private readonly IBLService _blService;
        private readonly ILogger<TelegramController> _logger;

        public TelegramController(ITgService tgService, IBLService blService,
            ILogger<TelegramController> logger)
        {
            _tgService = tgService;
            _blService = blService;
            _logger = logger;
        }

        //[HttpPost("webhook")]
        //public async Task<string> WebhookAsync(dynamic bindings)
        //{
        //    //var logger = NLog.LogManager.GetCurrentClassLogger();

        //    //string bindingsJson = JsonSerializer.Serialize(bindings);

        //    //logger.Info("bindingsJson: " + bindingsJson);

        //    //int chatId = bindings.Message.Chat.Id;
        //    //string promt = bindings.Message.Text;

        //    //if (promt == "/testpay")
        //    //{
        //    //await _tgService.SendTestInvoiceAsync(chatId);
        //    // await _tgService.SendTestInvoiceAsync(343585758);
        //    //}
        //    //else if (promt.StartsWith("/"))
        //    //{

        //    //}
        //    //else
        //    //{
        //    //    await _sunoService.GenerateMusicAsync(chatId, promt);
        //    //}

        //    await Task.Yield();
        //    return $"ok";
        //}

        [HttpPost("webhook")]
        public async Task<string> WebhookAsync(WebhookBinding bindings, CancellationToken cancellationToken)
        {
            try
            {
                if (bindings.Message != null)
                {
                    if (bindings.Message.Text != null)
                    {
                        var message = bindings.Message;
                        await _blService.ProcessTheMessageAsync(message, cancellationToken);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("WebhookAsync Exc: {ex}", ex);
            }

            return "ok";
        }
    }
}
