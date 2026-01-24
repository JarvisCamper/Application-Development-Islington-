using JournalApp.Models.Tag;

namespace JournalApp.Models.JournalEntry
{
    /// <summary>
    /// Light journal entry for list/timeline views
    /// </summary>
    public class JournalEntryListDto
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public string Title { get; set; } = string.Empty;
        public string ContentPreview => Content.Length > 150 ? Content.Substring(0, 150) + "..." : Content;
        public string Content { get; set; } = string.Empty;
        public string PrimaryMoodName { get; set; } = string.Empty;
        public string PrimaryMoodIcon { get; set; } = string.Empty;
        public string PrimaryMoodColor { get; set; } = string.Empty;
        public int TagCount { get; set; }
        public List<TagDto> Tags { get; set; } = new();
        public DateTime UpdatedAt { get; set; }
    }
}
