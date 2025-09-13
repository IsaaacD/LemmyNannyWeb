using LemmyWeb.Controllers;

namespace LemmyWeb.Models
{
    public class Processed
    {
        public int PostId { get; set; }
        public int Id { get; set; }
        public string? Url { get; set; }
        public string? Reason { get; set; }
        public string? Content { get; set; }
        public string? Title { get; set; }
        public bool IsReported { get; set; }
        public List<string> History { get; set; } = [];
        public ProcessedType ProcessedType { get; set; }
        public string? Username { get; set; }
        public string? AvatarUrl { get; set; }
        public DateTime ProcessedOn { get; set; }
        public string? CreatedDate { get; set; }
        public string? PostUrl { get; set; }
        public string? ExtraInfo { get; set; }
        public string? CommunityName { get; set; }
        public int WordCount { get; set; }
        public string? ThumbnailUrl { get; set; }
        public string? CommentNumber { get; set; }
        public bool Failed { get; set; }
        public bool ViewedImages { get; set; }
    }


    public enum ProcessedType
    {
        NotSet,
        Comment,
        Post
    }
}
