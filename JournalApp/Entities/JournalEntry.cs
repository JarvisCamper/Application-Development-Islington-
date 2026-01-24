namespace JournalApp.Entities
{
    /// Represents a journal entry for a specific date
    /// Only one entry is allowed per day

    public class JournalEntry : BaseEntity
    {
        public DateTime Date { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        
        // Mood relationships - 1 primary + up to 2 secondary
        public int PrimaryMoodId { get; set; }
        public Mood PrimaryMood { get; set; } = null!;
        
        public int? SecondaryMood1Id { get; set; }
        public Mood? SecondaryMood1 { get; set; }
        
        public int? SecondaryMood2Id { get; set; }
        public Mood? SecondaryMood2 { get; set; }
        
        // Category relationship
        public int? CategoryId { get; set; }
        public Category? Category { get; set; }
        
        // Tags - many-to-many relationship
        public ICollection<Tag> Tags { get; set; } = new List<Tag>();
    }
}
