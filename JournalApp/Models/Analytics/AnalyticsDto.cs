namespace JournalApp.Models.Analytics
{
    /// <summary>
    /// Comprehensive analytics data
    /// </summary>
    public class AnalyticsDto
    {
        public StreakDto Streak { get; set; } = new();
        public List<MoodDistributionDto> MoodDistribution { get; set; } = new();
        public List<TagUsageStatsDto> TagUsage { get; set; } = new();
        public List<WordCountTrendDto> WordCountTrends { get; set; } = new();
        public string MostFrequentMood { get; set; } = string.Empty;
        public string MostUsedTag { get; set; } = string.Empty;
    }
}
