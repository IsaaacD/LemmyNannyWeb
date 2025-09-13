namespace LemmyWeb.Models
{
    public class LemmNannyStats
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
        public int PostsProcessed { get; set; }
        public int PostsFlagged { get; set; }
        public int CommentsProcessed { get; set; }
        public int CommentsFlagged { get; set; }
    }
}
