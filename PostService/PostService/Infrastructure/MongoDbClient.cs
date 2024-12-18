using Microsoft.Extensions.Options;
using MongoDB.Driver;
using PostService.Models;

namespace PostService.Infrastructure
{
    public class MongoDbClient
    {
        private readonly IMongoDatabase _database;

        public MongoDbClient(IOptions<MongoDbSettings> settings)
        {
            var client = new MongoClient(settings.Value.ConnectionString);
            _database = client.GetDatabase(settings.Value.DatabaseName);
        }

        public IMongoCollection<Post> Posts => _database.GetCollection<Post>("Posts");
    }
}
