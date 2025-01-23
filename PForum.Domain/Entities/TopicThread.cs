namespace PForum.Domain.Entities
{
    public class TopicThread
    {
        public Guid Id { get; set; }
        public string ThreadName { get; set; } = string.Empty;
        public string ThreadDescription { get; set; } = string.Empty;
        public Guid TopicId { get; set; }
        public Topic Topic { get; set; }
        public IEnumerable<TopicThreadMessage> Messages { get; set; }
        public Guid UserId { get; set; }
        public User User { get; set; }
        public DateTime PostedAt { get; set; }
    }
}
