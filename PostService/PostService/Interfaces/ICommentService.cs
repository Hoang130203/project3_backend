using PostService.Models;

namespace PostService.Interfaces
{
    public interface ICommentService
    {
        Task<Comment> AddCommentAsync(string postId, Comment comment);
        Task<Comment> ReplyToCommentAsync(string postId, string parentCommentId, Comment reply);
        Task UpdateCommentAsync(string postId, string commentId, string newContent);
        Task DeleteCommentAsync(string postId, string commentId);
        Task<IEnumerable<Comment>> GetCommentsAsync(string postId, int skip, int take);
    }
}
