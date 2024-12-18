namespace PostService.Models.Request
{
    public class CreateShareRequest
    {
        public string? Message { get; set; }
        public VisibilitySettings? VisibilitySettings { get; set; }
    }
}
