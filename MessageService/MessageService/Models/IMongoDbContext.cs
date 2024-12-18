using MongoDB.Driver;

namespace MessageService.Models
{
    public interface IMongoDbContext
    {
        IMongoCollection<Message> Messages { get; }
        IMongoCollection<Conversation> Conversations { get; }
    }
}
