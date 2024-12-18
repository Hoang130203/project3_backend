using PostService.Models;

namespace PostService.Interfaces
{
    public interface IAnalyticsService
    {
        Task TrackViewAsync(string postId, Guid userId);
        Task UpdateMetricsAsync(string postId);
        Task<PostMetrics> GetPostMetricsAsync(string postId);
        Task<EngagementReport> GenerateEngagementReportAsync(string postId);
    }
}
