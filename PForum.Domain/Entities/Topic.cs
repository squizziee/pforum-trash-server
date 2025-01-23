namespace PForum.Domain.Entities
{
    public class Topic
    {
        public Guid Id { get; set; }
        public string TopicName { get; set; } = string.Empty;
        public string TopicDescription { get; set; } = string.Empty;
        public Guid LanguageTopicId { get; set; }
        public LanguageTopic LanguageTopic{ get; set; }
        public IEnumerable<TopicThread> TopicThreads { get; set; }
    }
}
