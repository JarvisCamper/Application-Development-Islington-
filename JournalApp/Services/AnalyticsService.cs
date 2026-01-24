using JournalApp.Data;
using JournalApp.Models.Analytics;
using JournalApp.Models.Common;
using JournalApp.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace JournalApp.Services
{
    public class AnalyticsService : IAnalyticsService
    {
        private readonly AppDbContext _context;
        private readonly Microsoft.Extensions.Caching.Memory.IMemoryCache _cache;
        private const string CacheKey = "analytics_data";

        public AnalyticsService(AppDbContext context, Microsoft.Extensions.Caching.Memory.IMemoryCache cache)
        {
            _context = context;
            _cache = cache;
        }

        public async Task<AnalyticsDto> GetAnalyticsAsync(DateRangeDto? dateRange = null)
        {
            // Only cache if no date range is specified (default view)
            if (dateRange == null)
            {
                return await _cache.GetOrCreateAsync(CacheKey, async entry =>
                {
                    entry.SetAbsoluteExpiration(TimeSpan.FromMinutes(30));
                    return await FetchAnalyticsInternal(null);
                }) ?? new AnalyticsDto();
            }
            
            return await FetchAnalyticsInternal(dateRange);
        }

        private async Task<AnalyticsDto> FetchAnalyticsInternal(DateRangeDto? dateRange)
        {
            var streak = await GetStreakInfoAsync();
            var moodDist = await GetMoodDistributionAsync(dateRange);
            var tagUsage = await GetTagUsageStatsAsync(dateRange);
            var wordTrends = await GetWordCountTrendsAsync(dateRange);

            return new AnalyticsDto
            {
                Streak = streak,
                MoodDistribution = moodDist.ToList(),
                TagUsage = tagUsage.ToList(),
                WordCountTrends = wordTrends.ToList(),
                MostFrequentMood = moodDist.OrderByDescending(m => m.Count).FirstOrDefault()?.MoodName ?? "",
                MostUsedTag = tagUsage.OrderByDescending(t => t.UsageCount).FirstOrDefault()?.TagName ?? ""
            };
        }

        public async Task<StreakDto> GetStreakInfoAsync()
        {
            var entries = await _context.JournalEntries.OrderBy(e => e.Date).ToListAsync();

            if (!entries.Any())
            {
                return new StreakDto
                {
                    CurrentStreak = 0,
                    LongestStreak = 0,
                    TotalEntries = 0,
                    MissedDays = 0
                };
            }

            var totalEntries = entries.Count;
            var firstDate = entries.First().Date;
            var lastDate = entries.Last().Date;

            // Calculate current streak (from today backwards)
            int currentStreak = 0;
            var checkDate = DateTime.Today;
            while (entries.Any(e => e.Date.Date == checkDate.Date))
            {
                currentStreak++;
                checkDate = checkDate.AddDays(-1);
            }

            // Calculate longest streak
            int longestStreak = 0;
            int tempStreak = 1;
            
            for (int i = 1; i < entries.Count; i++)
            {
                var daysDiff = (entries[i].Date.Date - entries[i - 1].Date.Date).Days;
                
                if (daysDiff == 1)
                {
                    tempStreak++;
                }
                else
                {
                    longestStreak = Math.Max(longestStreak, tempStreak);
                    tempStreak = 1;
                }
            }
            longestStreak = Math.Max(longestStreak, tempStreak);

            // Calculate missed days
            var totalDaysSinceFirst = (DateTime.Today - firstDate.Date).Days + 1;
            var missedDays = totalDaysSinceFirst - totalEntries;

            return new StreakDto
            {
                CurrentStreak = currentStreak,
                LongestStreak = longestStreak,
                TotalEntries = totalEntries,
                MissedDays = Math.Max(0, missedDays),
                FirstEntryDate = firstDate,
                LastEntryDate = lastDate
            };
        }

        public async Task<IEnumerable<MoodDistributionDto>> GetMoodDistributionAsync(DateRangeDto? dateRange = null)
        {
            var entries = await GetEntriesInRange(dateRange);
            
            if (!entries.Any())
                return new List<MoodDistributionDto>();

            var totalEntries = entries.Count;
            var moodCounts = new Dictionary<int, (string Name, string Color, MoodCategory Category, int Count)>();

            foreach (var entry in entries)
            {
                // Count primary mood
                if (!moodCounts.ContainsKey(entry.PrimaryMoodId))
                {
                    moodCounts[entry.PrimaryMoodId] = (entry.PrimaryMood.Name, entry.PrimaryMood.Color, entry.PrimaryMood.Category, 0);
                }
                moodCounts[entry.PrimaryMoodId] = (
                    moodCounts[entry.PrimaryMoodId].Name,
                    moodCounts[entry.PrimaryMoodId].Color,
                    moodCounts[entry.PrimaryMoodId].Category,
                    moodCounts[entry.PrimaryMoodId].Count + 1
                );

                // Count secondary moods
                if (entry.SecondaryMood1Id.HasValue && entry.SecondaryMood1 != null)
                {
                    if (!moodCounts.ContainsKey(entry.SecondaryMood1Id.Value))
                    {
                        moodCounts[entry.SecondaryMood1Id.Value] = (entry.SecondaryMood1.Name, entry.SecondaryMood1.Color, entry.SecondaryMood1.Category, 0);
                    }
                    moodCounts[entry.SecondaryMood1Id.Value] = (
                        moodCounts[entry.SecondaryMood1Id.Value].Name,
                        moodCounts[entry.SecondaryMood1Id.Value].Color,
                        moodCounts[entry.SecondaryMood1Id.Value].Category,
                        moodCounts[entry.SecondaryMood1Id.Value].Count + 1
                    );
                }

                if (entry.SecondaryMood2Id.HasValue && entry.SecondaryMood2 != null)
                {
                    if (!moodCounts.ContainsKey(entry.SecondaryMood2Id.Value))
                    {
                        moodCounts[entry.SecondaryMood2Id.Value] = (entry.SecondaryMood2.Name, entry.SecondaryMood2.Color, entry.SecondaryMood2.Category, 0);
                    }
                    moodCounts[entry.SecondaryMood2Id.Value] = (
                        moodCounts[entry.SecondaryMood2Id.Value].Name,
                        moodCounts[entry.SecondaryMood2Id.Value].Color,
                        moodCounts[entry.SecondaryMood2Id.Value].Category,
                        moodCounts[entry.SecondaryMood2Id.Value].Count + 1
                    );
                }
            }

            return moodCounts.Select(kvp => new MoodDistributionDto
            {
                MoodName = kvp.Value.Name,
                Category = kvp.Value.Category,
                Color = kvp.Value.Color,
                Count = kvp.Value.Count,
                Percentage = (kvp.Value.Count / (double)totalEntries) * 100
            }).OrderByDescending(m => m.Count);
        }

        public async Task<IEnumerable<TagUsageStatsDto>> GetTagUsageStatsAsync(DateRangeDto? dateRange = null)
        {
            var entries = await GetEntriesInRange(dateRange);
            
            if (!entries.Any())
                return new List<TagUsageStatsDto>();

            var totalEntries = entries.Count;
            var tagCounts = new Dictionary<int, (string Name, int Count)>();

            foreach (var entry in entries)
            {
                foreach (var tag in entry.Tags)
                {
                    if (!tagCounts.ContainsKey(tag.Id))
                    {
                        tagCounts[tag.Id] = (tag.Name, 0);
                    }
                    tagCounts[tag.Id] = (tagCounts[tag.Id].Name, tagCounts[tag.Id].Count + 1);
                }
            }

            return tagCounts.Select(kvp => new TagUsageStatsDto
            {
                TagId = kvp.Key,
                TagName = kvp.Value.Name,
                UsageCount = kvp.Value.Count,
                Percentage = (kvp.Value.Count / (double)totalEntries) * 100
            }).OrderByDescending(t => t.UsageCount);
        }

        public async Task<IEnumerable<WordCountTrendDto>> GetWordCountTrendsAsync(DateRangeDto? dateRange = null)
        {
            var entries = await GetEntriesInRange(dateRange);
            
            var trends = entries
                .GroupBy(e => e.Date.Date)
                .Select(g => new WordCountTrendDto
                {
                    Date = g.Key,
                    WordCount = g.Sum(e => e.Content.Split(new[] { ' ', '\n', '\r', '\t' }, StringSplitOptions.RemoveEmptyEntries).Length),
                    EntryCount = g.Count()
                })
                .OrderBy(t => t.Date);

            return trends;
        }

        private async Task<List<Entities.JournalEntry>> GetEntriesInRange(DateRangeDto? dateRange)
        {
            var query = _context.JournalEntries
                .Include(e => e.PrimaryMood)
                .Include(e => e.SecondaryMood1)
                .Include(e => e.SecondaryMood2)
                .Include(e => e.Tags)
                .AsQueryable();

            if (dateRange?.StartDate.HasValue == true)
            {
                query = query.Where(e => e.Date >= dateRange.StartDate.Value);
            }

            if (dateRange?.EndDate.HasValue == true)
            {
                query = query.Where(e => e.Date <= dateRange.EndDate.Value);
            }

            return await query.ToListAsync();
        }
    }
}
