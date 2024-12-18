namespace PostService.Models
{
    public class ContentModerationResult
    {
        public bool IsApproved { get; set; }
        public List<string> Violations { get; set; } = new();
        public double ConfidenceScore { get; set; }
        public Dictionary<string, double> CategoryScores { get; set; } = new();
    }
}
