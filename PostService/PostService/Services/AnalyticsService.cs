using MongoDB.Driver;
using PostService.Infrastructure;
using PostService.Interfaces;
using PostService.Models;
using SocialAppObjects.Enums;

namespace PostService.Services
{

    public class AnalyticsService : IAnalyticsService
    {
        private readonly IMongoCollection<Post> _posts;
        private readonly ILogger<AnalyticsService> _logger;

        public AnalyticsService(
            MongoDbClient mongoClient,
            ILogger<AnalyticsService> logger)
        {
            _posts = mongoClient.Posts;
            _logger = logger;
        }

        public async Task TrackViewAsync(string postId, Guid userId)
        {
            try
            {
                var update = Builders<Post>.Update.Combine(
                    Builders<Post>.Update.Inc("Metrics.ViewCount", 1),
                    //Builders<Post>.Update.AddToSet("Metrics.UniqueViewers", userId.ToString()),
                    Builders<Post>.Update.Set("Metrics.LastEngagementAt", DateTime.UtcNow)
                );

                await _posts.UpdateOneAsync(
                    Builders<Post>.Filter.Eq(p => p.Id, postId),
                    update
                );

                _logger.LogInformation("Tracked view for post {PostId} by user {UserId}", postId, userId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error tracking view for post {PostId}", postId);
                throw;
            }
        }

        public async Task UpdateMetricsAsync(string postId)
        {
            try
            {
                var post = await _posts.Find(p => p.Id == postId).FirstOrDefaultAsync();
                if (post == null)
                {
                    throw new KeyNotFoundException($"Post {postId} not found");
                }

                var metrics = new PostMetrics
                {
                    ViewCount = post.Metrics?.ViewCount ?? 0,
                    UniqueViewerCount = post.Metrics?.UniqueViewerCount ?? 0,
                    CommentCount = post.Comments?.Count ?? 0,
                    ShareCount = post.Shares?.Count ?? 0,
                    ReactionCounts = CalculateReactionCounts(post.Reactions),
                    EngagementRate = CalculateEngagementRate(post),
                    LastEngagementAt = DateTime.UtcNow
                };

                var update = Builders<Post>.Update.Set(p => p.Metrics, metrics);
                await _posts.UpdateOneAsync(
                    Builders<Post>.Filter.Eq(p => p.Id, postId),
                    update
                );

                _logger.LogInformation("Updated metrics for post {PostId}", postId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating metrics for post {PostId}", postId);
                throw;
            }
        }

        public async Task<PostMetrics> GetPostMetricsAsync(string postId)
        {
            try
            {
                var post = await _posts.Find(p => p.Id == postId)
                    .Project(p => p.Metrics)
                    .FirstOrDefaultAsync();

                if (post == null)
                {
                    throw new KeyNotFoundException($"Post {postId} not found");
                }

                return post;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving metrics for post {PostId}", postId);
                throw;
            }
        }

        public async Task<EngagementReport> GenerateEngagementReportAsync(string postId)
        {
            try
            {
                var post = await _posts.Find(p => p.Id == postId).FirstOrDefaultAsync();
                if (post == null)
                {
                    throw new KeyNotFoundException($"Post {postId} not found");
                }

                var report = new EngagementReport
                {
                    PostId = postId,
                    TimeToFirstEngagement = CalculateTimeToFirstEngagement(post),
                    EngagementsByHour = CalculateEngagementsByHour(post),
                    EngagementsByUserType = CalculateEngagementsByUserType(post),
                    ViralityMetrics = CalculateViralityMetrics(post)
                };

                _logger.LogInformation("Generated engagement report for post {PostId}", postId);
                return report;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating engagement report for post {PostId}", postId);
                throw;
            }
        }

        private Dictionary<ReactionType, int> CalculateReactionCounts(List<Reaction> reactions)
        {
            if (reactions == null || !reactions.Any())
                return new Dictionary<ReactionType, int>();

            return reactions
                .GroupBy(r => r.Type)
                .ToDictionary(
                    g => g.Key,
                    g => g.Count()
                );
        }

        private float CalculateEngagementRate(Post post)
        {
            if (post.Metrics?.ViewCount == 0)
                return 0;

            var totalEngagements = (post.Comments?.Count ?? 0) +
                                    (post.Reactions?.Count ?? 0) +
                                    (post.Shares?.Count ?? 0);

            return (float)totalEngagements / post.Metrics.ViewCount * 100;
        }

        private TimeSpan CalculateTimeToFirstEngagement(Post post)
        {
            var engagements = new List<DateTime>();

            if (post.Comments?.Any() == true)
                engagements.Add(post.Comments.Min(c => c.CreatedAt));
            if (post.Reactions?.Any() == true)
                engagements.Add(post.Reactions.Min(r => r.CreatedAt));
            if (post.Shares?.Any() == true)
                engagements.Add(post.Shares.Min(s => s.CreatedAt));

            if (!engagements.Any())
                return TimeSpan.Zero;

            return engagements.Min() - post.CreatedAt;
        }

        private Dictionary<string, int> CalculateEngagementsByHour(Post post)
        {
            var engagements = new Dictionary<string, int>();
            var allInteractions = new List<DateTime>();

            // Collect all engagement timestamps
            if (post.Comments?.Any() == true)
                allInteractions.AddRange(post.Comments.Select(c => c.CreatedAt));
            if (post.Reactions?.Any() == true)
                allInteractions.AddRange(post.Reactions.Select(r => r.CreatedAt));
            if (post.Shares?.Any() == true)
                allInteractions.AddRange(post.Shares.Select(s => s.CreatedAt));

            // Group by hour
            return allInteractions
                .GroupBy(dt => dt.ToString("yyyy-MM-dd HH:00"))
                .ToDictionary(
                    g => g.Key,
                    g => g.Count()
                );
        }

        private Dictionary<string, int> CalculateEngagementsByUserType(Post post)
        {
            // For now, return mock data
            return new Dictionary<string, int>
        {
            { "RegularUser", 10 },
            { "BusinessAccount", 5 },
            { "Guest", 2 }
        };
        }

        private List<ViralityMetric> CalculateViralityMetrics(Post post)
        {
            var metrics = new List<ViralityMetric>();
            var startTime = post.CreatedAt;
            var endTime = DateTime.UtcNow;

            for (var hour = startTime; hour <= endTime; hour = hour.AddHours(1))
            {
                var nextHour = hour.AddHours(1);
                var hourlyEngagements = CountEngagementsInTimeRange(post, hour, nextHour);
                var hourlyShares = CountSharesInTimeRange(post, hour, nextHour);

                metrics.Add(new ViralityMetric
                {
                    Timestamp = hour,
                    NewEngagements = hourlyEngagements,
                    NewShares = hourlyShares,
                    ViralityScore = CalculateHourlyViralityScore(hourlyEngagements, hourlyShares)
                });
            }

            return metrics;
        }

        private int CountEngagementsInTimeRange(Post post, DateTime start, DateTime end)
        {
            var count = 0;

            count += post.Comments?.Count(c => c.CreatedAt >= start && c.CreatedAt < end) ?? 0;
            count += post.Reactions?.Count(r => r.CreatedAt >= start && r.CreatedAt < end) ?? 0;
            count += post.Shares?.Count(s => s.CreatedAt >= start && s.CreatedAt < end) ?? 0;

            return count;
        }

        private int CountSharesInTimeRange(Post post, DateTime start, DateTime end)
        {
            return post.Shares?.Count(s => s.CreatedAt >= start && s.CreatedAt < end) ?? 0;
        }

        private double CalculateHourlyViralityScore(int engagements, int shares)
        {
            // Simple virality score calculation
            // Can be made more sophisticated based on requirements
            return (engagements * 0.3) + (shares * 0.7);
        }
    }
}
