namespace JournalApp.Models.Analytics
{
    /// <summary>
    /// Tag usage statistics for analytics
    /// </summary>
    public class TagUsageStatsDto
    {
        public int TagId { get; set; }
        public string TagName { get; set; } = string.Empty;
        public int UsageCount { get; set; }
        public double Percentage { get; set; }
    }
}
