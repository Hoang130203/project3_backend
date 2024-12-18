using Microsoft.Extensions.Options;
using MongoDB.Driver;
using ProfileService.Models;

namespace ProfileService.Infrastructure
{
    public class MongoDbClient
    {
        private readonly IMongoDatabase _database;

        public MongoDbClient(IOptions<MongoDbSettings> settings)
        {
            var client = new MongoClient(settings.Value.ConnectionString);
            _database = client.GetDatabase(settings.Value.DatabaseName);
        }
        public IMongoCollection<UserProfile> UserProfiles => _database.GetCollection<UserProfile>("UserProfiles");
        public IMongoCollection<GroupInfo> GroupInfos => _database.GetCollection<GroupInfo>("GroupInfos");
    }
}
