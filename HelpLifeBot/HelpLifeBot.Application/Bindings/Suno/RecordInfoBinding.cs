namespace HelpLifeBot.Bindings.Suno
{
    public class RecordInfoBinding
    {
        public RecordInfoData Data { get; set; } = null!;
    }

    public class RecordInfoData
    {
        public string Status { get; set; } = null!;
        public RecordInfoResponse Response { get; set; } = null!;
    }

    public class RecordInfoResponse
    {
        public List<RecordInfoTrack> SunoData { get; set; } = null!;
    }

    public class RecordInfoTrack
    {
        public string Title { get; set; } = null!;

        public string AudioUrl { get; set; } = null!;
    }
}
