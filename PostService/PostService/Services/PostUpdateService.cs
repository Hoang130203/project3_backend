using MongoDB.Driver;
using PostService.Infrastructure;
using PostService.Interfaces;
using PostService.Models;

namespace PostService.Services
{
    public class PostUpdateService : IPostUpdateService
    {
        private readonly IMongoCollection<Post> _posts;

        public PostUpdateService(MongoDbClient mongoClient)
        {
            _posts = mongoClient.Posts;
        }

        public async Task UpdateAsync(string postId, UpdateDefinition<Post> update)
        {
            var filter = Builders<Post>.Filter.Eq("_id", postId);
            await _posts.UpdateOneAsync(filter, update);
        }
    }
}
