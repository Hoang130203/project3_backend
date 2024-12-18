namespace PostService.Models
{
    public class MediaProcessingResult
    {
        public bool Success { get; set; }
        public string? ProcessedUrl { get; set; }
        public string? ThumbnailUrl { get; set; }
        public Dictionary<string, string> Metadata { get; set; } = new();
        public List<string> Errors { get; set; } = new();
    }
}
