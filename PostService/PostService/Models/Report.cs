using SocialAppObjects.Enums;

namespace PostService.Models
{
    public class Report
    {
        public string Id { get; set; }
        public User ReportedBy { get; set; }
        public ReportReason Reason { get; set; }
        public string Description { get; set; }
        public ReportStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime ResolvedAt { get; set; }
        public string? ModeratorNotes { get; set; }
    }
}
