using System.Text.Json.Serialization;

namespace HelpLifeBot
{
    public class SunoGenerateResponse
    {
        [JsonPropertyName("data")]
        public SunoGenerateData Data { get; set; } = null!;
    }

    public class SunoGenerateData
    {
        public string TaskId { get; set; } = null!;
    }
}
