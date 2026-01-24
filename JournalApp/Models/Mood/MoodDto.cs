using JournalApp.Enums;

namespace JournalApp.Models.Mood
{
    /// <summary>
    /// Full mood data for display
    /// </summary>
    public class MoodDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public MoodCategory Category { get; set; }
        public string Color { get; set; } = string.Empty;
        public string Icon { get; set; } = string.Empty;
    }
}
