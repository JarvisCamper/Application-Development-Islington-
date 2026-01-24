using JournalApp.Enums;

namespace JournalApp.Entities
{
  
    /// Represents a mood that can be associated with journal entries

    public class Mood : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public MoodCategory Category { get; set; }
        public string Color { get; set; } = "#000000";
        public string Icon { get; set; } = string.Empty;

        // Navigation properties
        public ICollection<JournalEntry> PrimaryMoodEntries { get; set; } = new List<JournalEntry>();
        public ICollection<JournalEntry> SecondaryMood1Entries { get; set; } = new List<JournalEntry>();
        public ICollection<JournalEntry> SecondaryMood2Entries { get; set; } = new List<JournalEntry>();
    }
}
