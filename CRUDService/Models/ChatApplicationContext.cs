using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace CRUDService.Models
{
    public partial class ChatapplicationContext : DbContext
    {
        public ChatapplicationContext()
        {
        }

        public ChatapplicationContext(DbContextOptions<ChatapplicationContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Conversations> Conversations { get; set; }
        public virtual DbSet<Messages> Messages { get; set; }
        public virtual DbSet<Users> Users { get; set; }
        public virtual DbSet<UsersConversations> UsersConversations { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseSqlServer("Server=localhost, 1434;Database=Chatapplication;User Id=sa;Password=8jkGh47hnDw89Haq8LN2;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("ProductVersion", "2.2.3-servicing-35854");

            modelBuilder.Entity<Conversations>(entity =>
            {
                entity.HasKey(e => e.ConversationId);

                entity.Property(e => e.ConversationId).HasDefaultValueSql("(newid())");
            });

            modelBuilder.Entity<Messages>(entity =>
            {
                entity.HasKey(e => e.MessageId);

                entity.Property(e => e.MessageId).HasDefaultValueSql("(newid())");

                entity.Property(e => e.Message)
                    .IsRequired()
                    .IsUnicode(false);

                entity.Property(e => e.TimeStamp)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.HasOne(d => d.Conversation)
                    .WithMany(p => p.Messages)
                    .HasForeignKey(d => d.ConversationId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Messages_Conversations");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Messages)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Messages_Users");
            });

            modelBuilder.Entity<Users>(entity =>
            {
                entity.HasKey(e => e.UserId);

                entity.Property(e => e.UserId).HasDefaultValueSql("(newid())");

                entity.Property(e => e.FirstName)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.LastName)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Picture).IsUnicode(false);

                entity.Property(e => e.Username)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<UsersConversations>(entity =>
            {
                entity.HasKey(e => new { e.UserId, e.ConversationId });

                entity.ToTable("Users_Conversations");

                entity.HasOne(d => d.Conversation)
                    .WithMany(p => p.UsersConversations)
                    .HasForeignKey(d => d.ConversationId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Users_Conversations_Conversations");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.UsersConversations)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Users_Conversations_Users");
            });
        }
    }
}
