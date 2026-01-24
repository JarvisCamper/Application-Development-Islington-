using JournalApp.Data;
using JournalApp.Entities;
using JournalApp.Models.Tag;
using Microsoft.EntityFrameworkCore;

namespace JournalApp.Services
{
    public class TagService : ITagService
    {
        private readonly AppDbContext _context;

        public TagService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<TagDto>> GetAllTagsAsync()
        {
            var tags = await _context.Tags.OrderBy(t => t.Name).ToListAsync();
            return tags.Select(MapToDto);
        }

        public async Task<TagDto> CreateCustomTagAsync(CreateTagDto dto)
        {
            if (await TagExistsAsync(dto.Name))
            {
                throw new InvalidOperationException($"Tag '{dto.Name}' already exists.");
            }

            var tag = new Tag
            {
                Name = dto.Name,
                IsCustom = true
            };

            _context.Tags.Add(tag);
            await _context.SaveChangesAsync();

            return MapToDto(tag);
        }

        public async Task<IEnumerable<TagDto>> GetTagsByIdsAsync(int[] ids)
        {
            var tags = await _context.Tags
                .Where(t => ids.Contains(t.Id))
                .ToListAsync();
            return tags.Select(MapToDto);
        }

        public async Task<bool> TagExistsAsync(string name)
        {
            return await _context.Tags.AnyAsync(t => t.Name.ToLower() == name.ToLower());
        }

        private static TagDto MapToDto(Tag tag)
        {
            return new TagDto
            {
                Id = tag.Id,
                Name = tag.Name,
                IsCustom = tag.IsCustom
            };
        }
    }
}
