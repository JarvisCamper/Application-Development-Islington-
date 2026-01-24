using JournalApp.Entities;
using JournalApp.Enums;
using Microsoft.EntityFrameworkCore;

namespace JournalApp.Data
{
    /// <summary>
    /// Database context for the Journal application
    /// </summary>
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<JournalEntry> JournalEntries => Set<JournalEntry>();
        public DbSet<Mood> Moods => Set<Mood>();
        public DbSet<Tag> Tags => Set<Tag>();
        public DbSet<Category> Categories => Set<Category>();
        public DbSet<UserSecurity> UserSecurities => Set<UserSecurity>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure JournalEntry
            modelBuilder.Entity<JournalEntry>(entity =>
            {
                entity.HasKey(e => e.Id);
                
                // Unique constraint on Date - only one entry per day
                entity.HasIndex(e => e.Date).IsUnique();
                
                entity.Property(e => e.Title).HasMaxLength(200);
                entity.Property(e => e.Content).IsRequired();
                entity.Property(e => e.Date).IsRequired();

                // Primary Mood relationship (required)
                entity.HasOne(e => e.PrimaryMood)
                    .WithMany(m => m.PrimaryMoodEntries)
                    .HasForeignKey(e => e.PrimaryMoodId)
                    .OnDelete(DeleteBehavior.Restrict);

                // Secondary Mood 1 relationship (optional)
                entity.HasOne(e => e.SecondaryMood1)
                    .WithMany(m => m.SecondaryMood1Entries)
                    .HasForeignKey(e => e.SecondaryMood1Id)
                    .OnDelete(DeleteBehavior.Restrict);

                // Secondary Mood 2 relationship (optional)
                entity.HasOne(e => e.SecondaryMood2)
                    .WithMany(m => m.SecondaryMood2Entries)
                    .HasForeignKey(e => e.SecondaryMood2Id)
                    .OnDelete(DeleteBehavior.Restrict);

                // Category relationship (optional)
                entity.HasOne(e => e.Category)
                    .WithMany(c => c.JournalEntries)
                    .HasForeignKey(e => e.CategoryId)
                    .OnDelete(DeleteBehavior.SetNull);

                // Many-to-many relationship with Tags
                entity.HasMany(e => e.Tags)
                    .WithMany(t => t.JournalEntries)
                    .UsingEntity(j => j.ToTable("JournalEntryTags"));
            });

            // Configure Mood
            modelBuilder.Entity<Mood>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Color).HasMaxLength(7); // Hex color code
                entity.Property(e => e.Icon).HasMaxLength(50);
            });

            // Configure Tag
            modelBuilder.Entity<Tag>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(50);
                entity.HasIndex(e => e.Name).IsUnique();
            });

            // Configure Category
            modelBuilder.Entity<Category>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Description).HasMaxLength(500);
            });

            // Configure UserSecurity
            modelBuilder.Entity<UserSecurity>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.PasswordHash).IsRequired();
                entity.Property(e => e.Salt).IsRequired();
            });

            // Seed data
            SeedData(modelBuilder);
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            // Auto-set timestamps
            var entries = ChangeTracker.Entries()
                .Where(e => e.Entity is BaseEntity && (e.State == EntityState.Added || e.State == EntityState.Modified));

            foreach (var entry in entries)
            {
                var entity = (BaseEntity)entry.Entity;
                
                if (entry.State == EntityState.Added)
                {
                    entity.CreatedAt = DateTime.UtcNow;
                }
                
                entity.UpdatedAt = DateTime.UtcNow;
            }

            return base.SaveChangesAsync(cancellationToken);
        }

        private void SeedData(ModelBuilder modelBuilder)
        {
            // Seed Moods
            modelBuilder.Entity<Mood>().HasData(
                // Positive moods (1-5)
                new Mood { Id = 1, Name = "Happy", Category = MoodCategory.Positive, Color = "#FFD700", Icon = "😊", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                new Mood { Id = 2, Name = "Excited", Category = MoodCategory.Positive, Color = "#FF6347", Icon = "🤩", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                new Mood { Id = 3, Name = "Relaxed", Category = MoodCategory.Positive, Color = "#87CEEB", Icon = "😌", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                new Mood { Id = 4, Name = "Grateful", Category = MoodCategory.Positive, Color = "#98FB98", Icon = "🙏", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                new Mood { Id = 5, Name = "Confident", Category = MoodCategory.Positive, Color = "#FFA500", Icon = "💪", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },

                // Neutral moods (6-10)
                new Mood { Id = 6, Name = "Calm", Category = MoodCategory.Neutral, Color = "#B0C4DE", Icon = "😐", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                new Mood { Id = 7, Name = "Thoughtful", Category = MoodCategory.Neutral, Color = "#DDA0DD", Icon = "🤔", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                new Mood { Id = 8, Name = "Curious", Category = MoodCategory.Neutral, Color = "#F0E68C", Icon = "🧐", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                new Mood { Id = 9, Name = "Nostalgic", Category = MoodCategory.Neutral, Color = "#D8BFD8", Icon = "🥺", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                new Mood { Id = 10, Name = "Bored", Category = MoodCategory.Neutral, Color = "#A9A9A9", Icon = "😑", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },

                // Negative moods (11-15)
                new Mood { Id = 11, Name = "Sad", Category = MoodCategory.Negative, Color = "#4682B4", Icon = "😢", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                new Mood { Id = 12, Name = "Angry", Category = MoodCategory.Negative, Color = "#DC143C", Icon = "😠", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                new Mood { Id = 13, Name = "Stressed", Category = MoodCategory.Negative, Color = "#FF4500", Icon = "😰", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                new Mood { Id = 14, Name = "Lonely", Category = MoodCategory.Negative, Color = "#708090", Icon = "😔", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                new Mood { Id = 15, Name = "Anxious", Category = MoodCategory.Negative, Color = "#8B4513", Icon = "😟", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow }
            );

            // Seed Tags
            modelBuilder.Entity<Tag>().HasData(
                new Tag { Id = 1, Name = "Work", IsCustom = false, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                new Tag { Id = 2, Name = "Health", IsCustom = false, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                new Tag { Id = 3, Name = "Travel", IsCustom = false, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                new Tag { Id = 4, Name = "Fitness", IsCustom = false, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                new Tag { Id = 5, Name = "Family", IsCustom = false, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                new Tag { Id = 6, Name = "Friends", IsCustom = false, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                new Tag { Id = 7, Name = "Hobby", IsCustom = false, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                new Tag { Id = 8, Name = "Learning", IsCustom = false, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                new Tag { Id = 9, Name = "Finance", IsCustom = false, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                new Tag { Id = 10, Name = "Personal", IsCustom = false, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow }
            );

            // Seed Categories
            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "Personal", Description = "Personal thoughts and reflections", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                new Category { Id = 2, Name = "Work", Description = "Work-related entries", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                new Category { Id = 3, Name = "Goals", Description = "Goal tracking and progress", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                new Category { Id = 4, Name = "Gratitude", Description = "Things to be grateful for", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                new Category { Id = 5, Name = "Ideas", Description = "Creative ideas and brainstorming", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow }
            );
        }
    }
}
