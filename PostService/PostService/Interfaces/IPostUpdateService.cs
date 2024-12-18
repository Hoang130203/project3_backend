using MongoDB.Driver;
using PostService.Models;

namespace PostService.Interfaces
{
    public interface IPostUpdateService
    {
        Task UpdateAsync(string postId, UpdateDefinition<Post> update);
    }
}
