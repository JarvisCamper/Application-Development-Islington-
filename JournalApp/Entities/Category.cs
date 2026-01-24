namespace JournalApp.Entities
{
 
    /// Represents a category for organizing journal entries
    
    public class Category : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        // Navigation properties
        public ICollection<JournalEntry> JournalEntries { get; set; } = new List<JournalEntry>();
    }
}
