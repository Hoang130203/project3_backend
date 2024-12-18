namespace MessageService.Models
{
    public class MessageDbSettings
    {
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }
        public string MessagesCollection { get; set; }
        public string ConversationsCollection { get; set; }
    }
}
