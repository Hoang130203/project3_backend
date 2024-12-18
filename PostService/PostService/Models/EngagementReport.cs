namespace PostService.Models
{
    public class EngagementReport
    {
        public string PostId { get; set; }
        public TimeSpan TimeToFirstEngagement { get; set; }
        public Dictionary<string, int> EngagementsByHour { get; set; } = new();
        public Dictionary<string, int> EngagementsByUserType { get; set; } = new();
        public List<ViralityMetric> ViralityMetrics { get; set; } = new();
    }

    public class ViralityMetric
    {
        public DateTime Timestamp { get; set; }
        public int NewEngagements { get; set; }
        public int NewShares { get; set; }
        public double ViralityScore { get; set; }
    }
}
