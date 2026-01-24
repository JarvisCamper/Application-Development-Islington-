namespace JournalApp.Entities
{
 
    /// Represents a tag that can be applied to journal entries

    public class Tag : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public bool IsCustom { get; set; }

        // Navigation properties
        public ICollection<JournalEntry> JournalEntries { get; set; } = new List<JournalEntry>();
    }
}
