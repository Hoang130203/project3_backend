using PostService.Models;
using SocialAppObjects.Enums;

namespace PostService.Interfaces
{
    public interface IPostModerationService
    {
        Task<ContentModerationResult> ModerateContentAsync(Post post);
        Task<Report> CreateReportAsync(string postId, Report report);
        Task<Report> ReviewReportAsync(string reportId, ReportStatus newStatus, string moderatorNotes);
        //Task<IEnumerable<Post>> GetReportedPostsAsync(int skip, int take);
    }
}
