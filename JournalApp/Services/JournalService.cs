using JournalApp.Data;
using JournalApp.Entities;
using JournalApp.Models.Category;
using JournalApp.Models.Common;
using JournalApp.Models.JournalEntry;
using JournalApp.Models.Mood;
using JournalApp.Models.Tag;
using Microsoft.EntityFrameworkCore;

namespace JournalApp.Services
{
    /// <summary>
    /// Service for managing journal entries with CRUD, validation, and search
    /// </summary>
    public class JournalService : IJournalService
    {
        private readonly AppDbContext _context;
        private readonly Microsoft.Extensions.Caching.Memory.IMemoryCache _cache;
        private const string AnalyticsCacheKey = "analytics_data";

        public JournalService(AppDbContext context, Microsoft.Extensions.Caching.Memory.IMemoryCache cache)
        {
            _context = context;
            _cache = cache;
        }

        public async Task<JournalEntryDto?> GetEntryByDateAsync(DateTime date)
        {
            var entry = await _context.JournalEntries
                .Include(e => e.PrimaryMood)
                .Include(e => e.SecondaryMood1)
                .Include(e => e.SecondaryMood2)
                .Include(e => e.Category)
                .Include(e => e.Tags)
                .FirstOrDefaultAsync(e => e.Date.Date == date.Date);

            return entry == null ? null : MapToDto(entry);
        }

        public async Task<JournalEntryDto?> GetEntryByIdAsync(int id)
        {
            var entry = await _context.JournalEntries
                .Include(e => e.PrimaryMood)
                .Include(e => e.SecondaryMood1)
                .Include(e => e.SecondaryMood2)
                .Include(e => e.Category)
                .Include(e => e.Tags)
                .FirstOrDefaultAsync(e => e.Id == id);

            return entry == null ? null : MapToDto(entry);
        }

        public async Task<PaginatedResult<JournalEntryListDto>> GetEntriesAsync(PaginationDto pagination)
        {
            var query = _context.JournalEntries
                .Include(e => e.PrimaryMood)
                .Include(e => e.Tags)
                .OrderByDescending(e => e.Date)
                .AsQueryable();

            int totalCount = await query.CountAsync();

            var entries = await query
                .Skip((pagination.PageNumber - 1) * pagination.PageSize)
                .Take(pagination.PageSize)
                .ToListAsync();

            var dtos = entries.Select(e => new JournalEntryListDto
            {
                Id = e.Id,
                Date = e.Date,
                Title = e.Title,
                Content = e.Content,
                PrimaryMoodName = e.PrimaryMood.Name,
                PrimaryMoodIcon = e.PrimaryMood.Icon,
                PrimaryMoodColor = e.PrimaryMood.Color,
                TagCount = e.Tags.Count,
                Tags = e.Tags.Select(t => new TagDto { Id = t.Id, Name = t.Name, IsCustom = t.IsCustom }).ToList(),
                UpdatedAt = e.UpdatedAt
            });

            return new PaginatedResult<JournalEntryListDto>
            {
                Items = dtos,
                TotalCount = totalCount,
                PageNumber = pagination.PageNumber,
                PageSize = pagination.PageSize
            };
        }

        public async Task<JournalEntryDto> CreateEntryAsync(CreateJournalEntryDto dto)
        {
            if (!await CanCreateEntryForDateAsync(dto.Date.Date))
            {
                throw new InvalidOperationException($"An entry already exists for {dto.Date.Date:yyyy-MM-dd}. Only one entry per day is allowed.");
            }

            var primaryMood = await _context.Moods.FindAsync(dto.PrimaryMoodId);
            if (primaryMood == null)
            {
                throw new InvalidOperationException($"Primary mood with ID {dto.PrimaryMoodId} not found.");
            }

            if (dto.SecondaryMood1Id.HasValue)
            {
                var mood1 = await _context.Moods.FindAsync(dto.SecondaryMood1Id.Value);
                if (mood1 == null) throw new InvalidOperationException($"Secondary mood 1 with ID {dto.SecondaryMood1Id} not found.");
            }

            if (dto.SecondaryMood2Id.HasValue)
            {
                var mood2 = await _context.Moods.FindAsync(dto.SecondaryMood2Id.Value);
                if (mood2 == null) throw new InvalidOperationException($"Secondary mood 2 with ID {dto.SecondaryMood2Id} not found.");
            }

            // Get tags
            var tags = new List<Tag>();
            if (dto.TagIds.Any())
            {
                tags = await _context.Tags.Where(t => dto.TagIds.Contains(t.Id)).ToListAsync();
            }

            var entry = new JournalEntry
            {
                Date = dto.Date.Date,
                Title = dto.Title,
                Content = dto.Content,
                PrimaryMoodId = dto.PrimaryMoodId,
                SecondaryMood1Id = dto.SecondaryMood1Id,
                SecondaryMood2Id = dto.SecondaryMood2Id,
                CategoryId = dto.CategoryId,
                Tags = tags
            };

            _context.JournalEntries.Add(entry);
            await _context.SaveChangesAsync();
            
            // Invalidate cache
            _cache.Remove(AnalyticsCacheKey);

            var createdEntry = await _context.JournalEntries
                .Include(e => e.PrimaryMood)
                .Include(e => e.SecondaryMood1)
                .Include(e => e.SecondaryMood2)
                .Include(e => e.Category)
                .Include(e => e.Tags)
                .FirstOrDefaultAsync(e => e.Id == entry.Id);

            if (createdEntry == null) createdEntry = entry;

            return MapToDto(createdEntry);
        }

        public async Task<JournalEntryDto> UpdateEntryAsync(int id, UpdateJournalEntryDto dto)
        {
            var entry = await _context.JournalEntries
                .Include(e => e.Tags)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (entry == null)
            {
                throw new InvalidOperationException($"Entry with ID {id} not found.");
            }

            // Validate moods
            var primaryMood = await _context.Moods.FindAsync(dto.PrimaryMoodId);
            if (primaryMood == null)
            {
                throw new InvalidOperationException($"Primary mood with ID {dto.PrimaryMoodId} not found.");
            }

            if (dto.SecondaryMood1Id.HasValue)
            {
                var mood1 = await _context.Moods.FindAsync(dto.SecondaryMood1Id.Value);
                if (mood1 == null) throw new InvalidOperationException($"Secondary mood 1 with ID {dto.SecondaryMood1Id} not found.");
            }

            if (dto.SecondaryMood2Id.HasValue)
            {
                var mood2 = await _context.Moods.FindAsync(dto.SecondaryMood2Id.Value);
                if (mood2 == null) throw new InvalidOperationException($"Secondary mood 2 with ID {dto.SecondaryMood2Id} not found.");
            }

            // Get tags
            var tags = new List<Tag>();
            if (dto.TagIds.Any())
            {
                tags = await _context.Tags.Where(t => dto.TagIds.Contains(t.Id)).ToListAsync();
            }

            // Update entry
            entry.Title = dto.Title;
            entry.Content = dto.Content;
            entry.PrimaryMoodId = dto.PrimaryMoodId;
            entry.SecondaryMood1Id = dto.SecondaryMood1Id;
            entry.SecondaryMood2Id = dto.SecondaryMood2Id;
            entry.CategoryId = dto.CategoryId;
            
            // Update tags (many-to-many)
            entry.Tags.Clear();
            foreach (var tag in tags)
            {
                entry.Tags.Add(tag);
            }

            _context.JournalEntries.Update(entry);
            await _context.SaveChangesAsync();
            
            // Invalidate cache
            _cache.Remove(AnalyticsCacheKey);

            // Reload for DTO
            var updatedEntry = await _context.JournalEntries
                .Include(e => e.PrimaryMood)
                .Include(e => e.SecondaryMood1)
                .Include(e => e.SecondaryMood2)
                .Include(e => e.Category)
                .Include(e => e.Tags)
                .FirstOrDefaultAsync(e => e.Id == id);

            return MapToDto(updatedEntry!);
        }

        public async Task DeleteEntryAsync(int id)
        {
            var entry = await _context.JournalEntries.FindAsync(id);
            if (entry == null)
            {
                throw new InvalidOperationException($"Entry with ID {id} not found.");
            }

            _context.JournalEntries.Remove(entry);
            await _context.SaveChangesAsync();
            
            // Invalidate cache
            _cache.Remove(AnalyticsCacheKey);
        }

        public async Task<bool> CanCreateEntryForDateAsync(DateTime date)
        {
            return !await _context.JournalEntries.AnyAsync(e => e.Date.Date == date.Date);
        }

        public async Task<PaginatedResult<JournalEntryListDto>> SearchEntriesAsync(SearchFilterDto filter)
        {
            var query = _context.JournalEntries
                .Include(e => e.PrimaryMood)
                .Include(e => e.Tags)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(filter.SearchTerm))
            {
                var term = filter.SearchTerm.ToLower();
                query = query.Where(e => e.Title.ToLower().Contains(term) || e.Content.ToLower().Contains(term));
            }

            if (filter.StartDate.HasValue)
            {
                query = query.Where(e => e.Date >= filter.StartDate.Value);
            }

            if (filter.EndDate.HasValue)
            {
                query = query.Where(e => e.Date <= filter.EndDate.Value);
            }

            if (filter.MoodIds != null && filter.MoodIds.Any())
            {
                query = query.Where(e => filter.MoodIds.Contains(e.PrimaryMoodId));
            }

            if (filter.TagIds != null && filter.TagIds.Any())
            {
                query = query.Where(e => e.Tags.Any(t => filter.TagIds.Contains(t.Id)));
            }

            int totalCount = await query.CountAsync();

            var entries = await query
                .OrderByDescending(e => e.Date)
                .Skip((filter.Pagination.PageNumber - 1) * filter.Pagination.PageSize)
                .Take(filter.Pagination.PageSize)
                .ToListAsync();

            var dtos = entries.Select(e => new JournalEntryListDto
            {
                Id = e.Id,
                Date = e.Date,
                Title = e.Title,
                Content = e.Content,
                PrimaryMoodName = e.PrimaryMood.Name,
                PrimaryMoodIcon = e.PrimaryMood.Icon,
                PrimaryMoodColor = e.PrimaryMood.Color,
                TagCount = e.Tags.Count,
                Tags = e.Tags.Select(t => new TagDto { Id = t.Id, Name = t.Name, IsCustom = t.IsCustom }).ToList(),
                UpdatedAt = e.UpdatedAt
            });

            return new PaginatedResult<JournalEntryListDto>
            {
                Items = dtos,
                TotalCount = totalCount,
                PageNumber = filter.Pagination.PageNumber,
                PageSize = filter.Pagination.PageSize
            };
        }

        private JournalEntryDto MapToDto(JournalEntry entry)
        {
            return new JournalEntryDto
            {
                Id = entry.Id,
                Date = entry.Date,
                Title = entry.Title,
                Content = entry.Content,
                PrimaryMood = new MoodDto
                {
                    Id = entry.PrimaryMood.Id,
                    Name = entry.PrimaryMood.Name,
                    Category = entry.PrimaryMood.Category,
                    Color = entry.PrimaryMood.Color,
                    Icon = entry.PrimaryMood.Icon
                },
                SecondaryMood1 = entry.SecondaryMood1 != null ? new MoodDto
                {
                    Id = entry.SecondaryMood1.Id,
                    Name = entry.SecondaryMood1.Name,
                    Category = entry.SecondaryMood1.Category,
                    Color = entry.SecondaryMood1.Color,
                    Icon = entry.SecondaryMood1.Icon
                } : null,
                SecondaryMood2 = entry.SecondaryMood2 != null ? new MoodDto
                {
                    Id = entry.SecondaryMood2.Id,
                    Name = entry.SecondaryMood2.Name,
                    Category = entry.SecondaryMood2.Category,
                    Color = entry.SecondaryMood2.Color,
                    Icon = entry.SecondaryMood2.Icon
                } : null,
                Category = entry.Category != null ? new CategoryDto
                {
                    Id = entry.Category.Id,
                    Name = entry.Category.Name,
                    Description = entry.Category.Description
                } : null,
                Tags = entry.Tags.Select(t => new TagDto
                {
                    Id = t.Id,
                    Name = t.Name,
                    IsCustom = t.IsCustom
                }).ToList(),
                CreatedAt = entry.CreatedAt,
                UpdatedAt = entry.UpdatedAt
            };
        }
    }
}
