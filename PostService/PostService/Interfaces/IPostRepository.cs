using MongoDB.Driver;
using PostService.Models;
using PostService.Services;

namespace PostService.Interfaces
{
    public interface IPostRepository
    {
        Task<Post> GetByIdAsync(string id);
        Task<IEnumerable<Post>> GetUserFeedAsync(Guid userId, int skip, int take);
        Task<IEnumerable<Post>> GetGroupPostsAsync(Guid groupId, int skip, int take);
        Task<string> CreateAsync(Post post);
        Task UpdateAsync(string id, Post post);
        Task DeleteAsync(string id);
        Task<IEnumerable<Post>> SearchAsync(PostSearchParams searchParams);
        Task<bool> ValidatePostContentAsync(Post post);
        Task UpdateMediaAsync(string postId, UpdateDefinition<Post> update);

        Task AddCommentAsync(string postId, Comment comment);
        Task AddReactionAsync(string postId, Reaction reaction);
        Task RemoveReactionAsync(string postId, string userId);
        Task AddShareAsync(string postId, Share share);

        Task<IEnumerable<FeedResult>> GetUserFeedAsync(Guid Id, int page);
        Task<IEnumerable<Post>> GetVideoPostsAsync(Guid userId, int page);
    }
}
