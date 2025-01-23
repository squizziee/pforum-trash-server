namespace PForum.Domain.Entities
{
    public class User
    {
        public Guid Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public IEnumerable<TopicThreadMessage> Messages { get; set; }
        public IEnumerable<TopicThread> CreatedThreads { get; set; }
        public UserRole Role { get; set; }
    }
}
