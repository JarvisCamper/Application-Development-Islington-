namespace JournalApp.Models.Mood
{
    /// <summary>
    /// Light mood data for selection controls
    /// </summary>
    public class MoodSummaryDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Color { get; set; } = string.Empty;
        public string Icon { get; set; } = string.Empty;
    }
}
