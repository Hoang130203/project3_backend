using PostService.Models;
using SocialAppObjects.Enums;

namespace PostService.Interfaces
{
    public interface IReactionService
    {
        Task AddReactionAsync(string postId, Reaction reaction);
        Task RemoveReactionAsync(string postId, string userId);
        Task<Dictionary<ReactionType, int>> GetReactionStatsAsync(string postId);
    }
}
