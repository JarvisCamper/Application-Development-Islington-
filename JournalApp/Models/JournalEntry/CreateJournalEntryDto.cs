using System.ComponentModel.DataAnnotations;

namespace JournalApp.Models.JournalEntry
{
    /// <summary>
    /// Input for creating a new journal entry
    /// </summary>
    public class CreateJournalEntryDto
    {
        [Required]
        public DateTime Date { get; set; } = DateTime.Today;

        [StringLength(100, ErrorMessage = "Title cannot exceed 100 characters.")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Content is required.")]
        public string Content { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please select a mood.")]
        [Range(1, int.MaxValue, ErrorMessage = "Please select a primary mood.")]
        public int PrimaryMoodId { get; set; }

        public int? SecondaryMood1Id { get; set; }
        public int? SecondaryMood2Id { get; set; }
        public int? CategoryId { get; set; }
        public List<int> TagIds { get; set; } = new();
    }
}
