using JournalApp.Models.Tag;

namespace JournalApp.Services
{
    /// <summary>
    /// Tag service for managing tags
    /// </summary>
    public interface ITagService
    {
        Task<IEnumerable<TagDto>> GetAllTagsAsync();
        Task<TagDto> CreateCustomTagAsync(CreateTagDto dto);
        Task<IEnumerable<TagDto>> GetTagsByIdsAsync(int[] ids);
        Task<bool> TagExistsAsync(string name);
    }
}
