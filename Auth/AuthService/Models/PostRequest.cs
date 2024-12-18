namespace AuthService.Models
{
    public class PostRequest
    {
        public string Content { get; set; } = string.Empty;
        public Guid GroupId { get; set; } = Guid.Empty;
    }
}
