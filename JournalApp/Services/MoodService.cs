using JournalApp.Data;
using JournalApp.Entities;
using JournalApp.Enums;
using JournalApp.Models.Mood;
using Microsoft.EntityFrameworkCore;

namespace JournalApp.Services
{
    public class MoodService : IMoodService
    {
        private readonly AppDbContext _context;

        public MoodService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<MoodDto>> GetAllMoodsAsync()
        {
            var moods = await _context.Moods.OrderBy(m => m.Id).ToListAsync();
            return moods.Select(MapToDto);
        }

        public async Task<IEnumerable<MoodDto>> GetMoodsByCategoryAsync(MoodCategory category)
        {
            var moods = await _context.Moods
                .Where(m => m.Category == category)
                .OrderBy(m => m.Id)
                .ToListAsync();
            return moods.Select(MapToDto);
        }

        public async Task<MoodDto?> GetMoodByIdAsync(int id)
        {
            var mood = await _context.Moods.FindAsync(id);
            return mood == null ? null : MapToDto(mood);
        }

        private static MoodDto MapToDto(Mood mood)
        {
            return new MoodDto
            {
                Id = mood.Id,
                Name = mood.Name,
                Category = mood.Category,
                Color = mood.Color,
                Icon = mood.Icon
            };
        }
    }
}
