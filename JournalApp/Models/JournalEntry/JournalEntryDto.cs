using JournalApp.Models.Category;
using JournalApp.Models.Mood;
using JournalApp.Models.Tag;

namespace JournalApp.Models.JournalEntry
{
    /// <summary>
    /// Full journal entry with all related data for display  
    /// </summary>
    public class JournalEntryDto
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        
        // Moods
        public MoodDto PrimaryMood { get; set; } = null!;
        public MoodDto? SecondaryMood1 { get; set; }
        public MoodDto? SecondaryMood2 { get; set; }
        
        // Category
        public CategoryDto? Category { get; set; }
        
        // Tags
        public List<TagDto> Tags { get; set; } = new();
        
        // Metadata
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public int WordCount => Content.Split(new[] { ' ', '\n', '\r', '\t' }, StringSplitOptions.RemoveEmptyEntries).Length;
    }
}
