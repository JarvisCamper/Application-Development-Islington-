using JournalApp.Models.Analytics;
using JournalApp.Models.Common;

namespace JournalApp.Services
{
    /// <summary>
    /// Analytics service for calculating statistics and trends
    /// </summary>
    public interface IAnalyticsService
    {
        Task<AnalyticsDto> GetAnalyticsAsync(DateRangeDto? dateRange = null);
        Task<StreakDto> GetStreakInfoAsync();
        Task<IEnumerable<MoodDistributionDto>> GetMoodDistributionAsync(DateRangeDto? dateRange = null);
        Task<IEnumerable<TagUsageStatsDto>> GetTagUsageStatsAsync(DateRangeDto? dateRange = null);
        Task<IEnumerable<WordCountTrendDto>> GetWordCountTrendsAsync(DateRangeDto? dateRange = null);
    }
}
