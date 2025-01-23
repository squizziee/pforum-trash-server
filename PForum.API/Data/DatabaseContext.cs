using Microsoft.EntityFrameworkCore;
using PForum.Domain.Entities;

namespace PForum.API.Data
{
    public class DatabaseContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<LanguageTopic> LanguageTopics { get; set; }
        public DbSet<Topic> Topics { get; set; }
        public DbSet<TopicThread> TopicThreads { get; set; }
        public DbSet<TopicThreadMessage> TopicThreadMessages { get; set; }

        public DatabaseContext(DbContextOptions<DatabaseContext> dbContextOptions) 
            : base(dbContextOptions) 
        {      
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // User
            modelBuilder.Entity<User>()
                .HasKey(u => u.Id);

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            // LanguageTopic
            modelBuilder.Entity<LanguageTopic>()
                .HasKey(lt => lt.Id);

            modelBuilder.Entity<LanguageTopic>()
                .HasMany(lt => lt.Topics)
                .WithOne(t => t.LanguageTopic)
                .HasForeignKey(t => t.LanguageTopicId);

            // Topic
            modelBuilder.Entity<Topic>()
                .HasKey(t => t.Id);

            modelBuilder.Entity<Topic>()
                .HasMany(t => t.TopicThreads)
                .WithOne(tt => tt.Topic)
                .HasForeignKey(tt => tt.TopicId);


            // TopicThread
            modelBuilder.Entity<TopicThread>()
                .HasKey(tt => tt.Id);

            modelBuilder.Entity<TopicThread>()
                .HasMany(tt => tt.Messages)
                .WithOne(ttm => ttm.TopicThread)
                .HasForeignKey(ttm => ttm.TopicThreadId);

            modelBuilder.Entity<TopicThread>()
                .HasOne(tt => tt.User)
                .WithMany(u => u.CreatedThreads)
                .HasForeignKey(tt => tt.UserId);

            // TopicThreadMessage
            modelBuilder.Entity<TopicThreadMessage>()
                .HasKey(ttm => ttm.Id);

            modelBuilder.Entity<TopicThreadMessage>()
                .HasOne(ttm => ttm.User)
                .WithMany(u => u.Messages)
                .HasForeignKey(ttm => ttm.UserId);
        }
    }
}
