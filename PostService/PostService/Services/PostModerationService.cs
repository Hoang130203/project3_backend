using MongoDB.Driver;
using PostService.Interfaces;
using PostService.Models;
using SocialAppObjects.Enums;
using System.Net.Http;
using System.Text.Json;
using System.Text;

namespace PostService.Services
{
    public class PostModerationService : IPostModerationService
    {
        private readonly ILogger<PostModerationService> _logger;
        private readonly IPostUpdateService _postUpdateService;
        private readonly HttpClient _httpClient;
        private const string OpenAIApiUrl = "https://api.openai.com/v1/completions";
        private const string OpenAIApiKey = "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"; // Thay bằng API key thật
        public PostModerationService(
            ILogger<PostModerationService> logger,
            IPostUpdateService postUpdateService
            )
        {
            _logger = logger;
            _postUpdateService = postUpdateService;
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {OpenAIApiKey}");
        }

        public async Task<ContentModerationResult> ModerateContentAsync(Post post)
        {
            try
            {
                var result = new ContentModerationResult
                {
                    IsApproved = true,
                    Violations = new List<string>(),
                    CategoryScores = new Dictionary<string, double>()
                };

                if (!string.IsNullOrEmpty(post.Content) &&
                    post.Content.Contains("spam", StringComparison.OrdinalIgnoreCase)
                    
                    )
                {
                    result.IsApproved = false;
                    result.Violations.Add("Potential spam detected");
                    result.CategoryScores["Spam"] = 0.8;
                }

                result.CategoryScores["Violence"] = 0.1;
                result.CategoryScores["Adult"] = 0.1;
                result.CategoryScores["Harassment"] = 0.1;

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during content moderation");
                return new ContentModerationResult
                {
                    IsApproved = true,
                    Violations = new List<string>()
                };
            }
        }


       

        public async Task<Report> CreateReportAsync(string postId, Report report)
        {
            report.Status = ReportStatus.Pending;
            report.CreatedAt = DateTime.UtcNow;

            var update = Builders<Post>.Update.Push(p => p.Reports, report);
            await _postUpdateService.UpdateAsync(postId, update);

            return report;
        }

        public async Task<Report> ReviewReportAsync(string reportId, ReportStatus newStatus, string moderatorNotes)
        {
            var report = new Report
            {
                Id = reportId,
                Status = newStatus,
                ModeratorNotes = moderatorNotes,
                ResolvedAt = DateTime.UtcNow
            };

            _logger.LogInformation(
                "Report {ReportId} reviewed with status {Status}",
                reportId,
                newStatus);

            return report;
        }
    }
}
