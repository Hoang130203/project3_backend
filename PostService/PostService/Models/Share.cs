namespace PostService.Models
{
    public class Share
    {
        public string ShareId { get; set; }
        public string UserId { get; set; }
        public string? Message { get; set; }
        public VisibilitySettings VisibilitySettings { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
