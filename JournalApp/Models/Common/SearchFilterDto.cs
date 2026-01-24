namespace JournalApp.Models.Common
{
    /// <summary>
    /// Search and filter criteria for journal entries
    /// </summary>
    public class SearchFilterDto
    {
        public string? SearchTerm { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public List<int> MoodIds { get; set; } = new();
        public List<int> TagIds { get; set; } = new();
        public int? CategoryId { get; set; }
        public PaginationDto Pagination { get; set; } = new();
    }
}
