using System.Text.Json.Serialization;

namespace HelpLifeBot.Bindings.Suno
{
    public class SunoCallbackBinding
    {
        public List<SunoCallbackTrack> Data { get; set; } = null!;
    }

    public class SunoCallbackTrack
    {
        [JsonPropertyName("title")]
        public string Title { get; set; } = null!;

        [JsonPropertyName("audio_url")]
        public string AudioUrl { get; set; } = null!;
    }
}
