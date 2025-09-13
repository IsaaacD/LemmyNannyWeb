namespace LemmyWeb.Models
{
    public class StartUpStats
    {
        public string Prompt { get; set; } = string.Empty;
        public string Model { get; set; } = string.Empty;
        public DateTime StartTime { get; set; }
        public bool ReadingMode { get; set; }
        public string SortType { get; set; } = string.Empty;
        public string ListingType { get; set; } = string.Empty;
        public string LemmyHost { get; set; } = string.Empty;
        public bool IsSet { get; set; }
        public DateTime LastSeen { get; set; }
    }
}
