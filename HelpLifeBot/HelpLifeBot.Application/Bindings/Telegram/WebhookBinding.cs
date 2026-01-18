using System.Text.Json.Serialization;
using Telegram.Bot.Types.Payments;

namespace HelpLifeBot.Bindings.Telegram
{
    public class WebhookBinding
    {
        public WebhookMessage? Message { get; set; }

        [JsonPropertyName("pre_checkout_query")]
        public WebhookPreCheckoutQuery? PreCheckoutQuery { get; set; }
    }

    public class WebhookMessage
    {
        public string? Text { get; set; }
        public WebhookMessageChat Chat { get; set; } = null!;
        public WebhookFromChat From { get; set; } = null!;

        [JsonPropertyName("successful_payment")]
        public WebhookSuccessfulPayment? SuccessfulPayment { get; set; }
    }

    public class WebhookMessageChat
    {
        public long Id { get; set; }
    }

    public class WebhookFromChat
    {
        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("is_bot")]
        public bool IsBot { get; set; }

        [JsonPropertyName("first_name")]
        public string FirstName { get; set; } = null!;

        [JsonPropertyName("last_name")]
        public string? LastName { get; set; }

        [JsonPropertyName("username")]
        public string? Username { get; set; }

        [JsonPropertyName("is_premium")]
        public bool IsPremium { get; set; }

        [JsonPropertyName("language_code")]
        public string? LanguageCode { get; set; }
    }

    public class WebhookPreCheckoutQuery
    {
        public string Id { get; set; } = null!;

        public WebhookFromChat From { get; set; } = null!;

        public string Currency { get; set; } = null!;

        [JsonPropertyName("total_amount")]
        public int TotalAmount { get; set; }

        [JsonPropertyName("invoice_payload")]
        public string InvoicePayload { get; set; } = null!;
    }

    public class WebhookSuccessfulPayment
    {
        public string Currency { get; set; } = null!;

        [JsonPropertyName("total_amount")]
        public int TotalAmount { get; set; }

        [JsonPropertyName("invoice_payload")]
        public string InvoicePayload { get; set; } = null!;

        [JsonPropertyName("provider_payment_charge_id")]
        public string ProviderPaymentChargeId { get; set; } = null!;

        [JsonPropertyName("telegram_payment_charge_id")]
        public string TelegramPaymentChargeId { get; set; } = null!;
    }
}
