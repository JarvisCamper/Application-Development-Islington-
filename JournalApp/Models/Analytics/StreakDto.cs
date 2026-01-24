namespace JournalApp.Models.Analytics
{
    /// <summary>
    /// Streak information for journal entries
    /// </summary>
    public class StreakDto
    {
        public int CurrentStreak { get; set; }
        public int LongestStreak { get; set; }
        public int TotalEntries { get; set; }
        public int MissedDays { get; set; }
        public DateTime? FirstEntryDate { get; set; }
        public DateTime? LastEntryDate { get; set; }
    }
}
