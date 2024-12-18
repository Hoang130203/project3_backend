namespace MessageService.Models
{
    public class Conversation
    {
        public Guid Id { get; set; }
        public List<Guid> ParticipantIds { get; set; } = new();
        public DateTime CreatedAt { get; set; }
        public DateTime LastMessageAt { get; set; }
    }


}
