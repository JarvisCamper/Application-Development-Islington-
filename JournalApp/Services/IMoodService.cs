using JournalApp.Models.Mood;
using JournalApp.Enums;

namespace JournalApp.Services
{
    /// <summary>
    /// Mood service for retrieving moods
    /// </summary>
    public interface IMoodService
    {
        Task<IEnumerable<MoodDto>> GetAllMoodsAsync();
        Task<IEnumerable<MoodDto>> GetMoodsByCategoryAsync(MoodCategory category);
        Task<MoodDto?> GetMoodByIdAsync(int id);
    }
}
