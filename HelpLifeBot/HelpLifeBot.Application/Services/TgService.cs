using Microsoft.Extensions.Configuration;
using System.Text;
using System.Text.Json;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace HelpLifeBot
{
    public interface ITgService
    {
        Task SendAudioFromUrlAsync(long chatId, string audioUrl, string caption = "");
        Task SendMessageAsync(long chatId, string text, string mode = "text");
        Task SendTestInvoiceAsync(long chatId);
        Task SendInvoiceAsync(long chatId, string title, string description, Guid payload, int priceAmount);
        Task SendAnswerPreCheckoutQueryAsync(string preCheckoutQueryId, bool ok, string? errorMessage = null);
    }

    public class TgService : ITgService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        private readonly NLog.Logger _logger;

        public TgService(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;

            _logger = NLog.LogManager.GetCurrentClassLogger();
        }

        public async Task SendAudioFromUrlAsync(long chatId, string audioUrl, string caption = "")
        {
            try
            {
                var botClient = new TelegramBotClient(_configuration.GetSection("Application:TelegramApi:ApiKey").Value!);
                var inputOnlineFile = new InputFileUrl(audioUrl);
                await botClient.SendAudio(
                    chatId: chatId,
                    audio: inputOnlineFile,
                    caption: caption
                );
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка отправки аудио: {ex.Message}");
            }
        }

        public async Task SendMessageAsync(long chatId, string text, string mode = "text")
        {
            using var client = _httpClientFactory.CreateClient(Consts.TelegramApi);

            var requestData = new
            {
                chat_id = chatId,
                text,
                //parse_mode = mode
            };

            var jsonPayload = JsonSerializer.Serialize(requestData);

            using var jsonContent = new StringContent(jsonPayload, Encoding.UTF8, "application/json");
            HttpResponseMessage response = await client.PostAsync("sendMessage", jsonContent);
            response.EnsureSuccessStatusCode();
        }

        public async Task SendTestInvoiceAsync(long chatId)
        {
            using var client = _httpClientFactory.CreateClient(Consts.TelegramApi);

            var requestData = new
            {
                chat_id = chatId,
                currency = "XTR",
                title = "Test zaplati",
                description = "Nu pzh",
                payload = "tst",
                prices = new [] { new { label = "two star", amount = 2 } }
            };

            var jsonPayload = JsonSerializer.Serialize(requestData);

            _logger.Info($"SendTestInvoiceAsync jsonPayload: {jsonPayload}");

            using var jsonContent = new StringContent(jsonPayload, Encoding.UTF8, "application/json");
            HttpResponseMessage response = await client.PostAsync("sendInvoice", jsonContent);
            response.EnsureSuccessStatusCode();

            var a = await response.Content.ReadAsStringAsync();
        }

        public async Task SendInvoiceAsync(long chatId, string title, string description, Guid payload, int priceAmount)
        {
            using var client = _httpClientFactory.CreateClient(Consts.TelegramApi);

            var requestData = new
            {
                chat_id = chatId,
                currency = "XTR",
                title,
                description,
                payload = $"{payload}",
                prices = new [] { new { label = $"{priceAmount} stars", amount = priceAmount } }
            };

            var jsonPayload = JsonSerializer.Serialize(requestData);

            using var jsonContent = new StringContent(jsonPayload, Encoding.UTF8, "application/json");
            HttpResponseMessage response = await client.PostAsync("sendInvoice", jsonContent);
            response.EnsureSuccessStatusCode();
        }

        public async Task SendAnswerPreCheckoutQueryAsync(string preCheckoutQueryId, bool ok, string? errorMessage = null)
        {
            using var client = _httpClientFactory.CreateClient(Consts.TelegramApi);

            var requestData = new
            {
                pre_checkout_query_id = preCheckoutQueryId,
                ok,
                error_message = errorMessage
            };

            var jsonPayload = JsonSerializer.Serialize(requestData);

            using var jsonContent = new StringContent(jsonPayload, Encoding.UTF8, "application/json");
            HttpResponseMessage response = await client.PostAsync("answerPreCheckoutQuery", jsonContent);
            response.EnsureSuccessStatusCode();
        }
    }
}
