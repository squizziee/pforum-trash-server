namespace PForum.Domain.Entities
{
    public class TopicThreadMessage
    {
        public Guid Id { get; set; }
        public string MessageText { get; set; } = string.Empty;
        public Guid TopicThreadId { get; set; }
        public TopicThread TopicThread { get; set; }
        public Guid UserId { get; set; }
        public User User { get; set; }
        public DateTime PostedAt { get; set; }
    }
}
