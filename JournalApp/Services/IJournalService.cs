using JournalApp.Models.Common;
using JournalApp.Models.JournalEntry;

namespace JournalApp.Services
{
    /// <summary>
    /// Journal entry service for CRUD operations and search/filter
    /// </summary>
    public interface IJournalService
    {
        Task<JournalEntryDto?> GetEntryByDateAsync(DateTime date);
        Task<JournalEntryDto?> GetEntryByIdAsync(int id);
        Task<PaginatedResult<JournalEntryListDto>> GetEntriesAsync(PaginationDto pagination);
        Task<JournalEntryDto> CreateEntryAsync(CreateJournalEntryDto dto);
        Task<JournalEntryDto> UpdateEntryAsync(int id, UpdateJournalEntryDto dto);
        Task DeleteEntryAsync(int id);
        Task<bool> CanCreateEntryForDateAsync(DateTime date);
        Task<PaginatedResult<JournalEntryListDto>> SearchEntriesAsync(SearchFilterDto filter);
    }
}
