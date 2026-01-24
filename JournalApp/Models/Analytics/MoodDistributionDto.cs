using JournalApp.Enums;

namespace JournalApp.Models.Analytics
{
    /// <summary>
    /// Mood distribution with counts and percentages
    /// </summary>
    public class MoodDistributionDto
    {
        public MoodCategory Category { get; set; }
        public string MoodName { get; set; } = string.Empty;
        public int Count { get; set; }
        public double Percentage { get; set; }
        public string Color { get; set; } = string.Empty;
    }
}
