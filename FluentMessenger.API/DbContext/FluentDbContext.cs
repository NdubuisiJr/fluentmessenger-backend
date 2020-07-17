using FluentMessenger.API.Entities;
using Microsoft.EntityFrameworkCore;

namespace FluentMessenger.API.DBContext {
    public class FluentDbContext : DbContext {
        public FluentDbContext() {
        }

        public FluentDbContext(DbContextOptions<FluentDbContext> options)
            : base(options) {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            modelBuilder.Entity<ContactMessagesNotReceived>().HasKey(cm => new { cm.ContactId, cm.MessageId });
            modelBuilder.Entity<ContactMessagesReceived>().HasKey(cm => new { cm.ContactId, cm.MessageId });

            modelBuilder.Entity<User>().HasOne(a => a.Sender)
                                       .WithOne(b => b.User)
                                       .HasForeignKey<Sender>(b => b.UserId);

            base.OnModelCreating(modelBuilder);
        }

        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<Group> Groups { get; set; }
        public virtual DbSet<Message> Messages { get; set; }
        public virtual DbSet<Contact> Contacts { get; set; }
        public virtual DbSet<MessageTemplate> MessageTemplates { get; set; }
        public virtual DbSet<ContactMessagesReceived> ContactMessagesReceived { get; set; }
        public virtual DbSet<ContactMessagesNotReceived> ContactMessagesNotReceived { get; set; }
    }
}
