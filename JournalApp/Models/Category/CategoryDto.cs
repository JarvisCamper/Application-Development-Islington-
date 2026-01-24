namespace JournalApp.Models.Category
{
    /// <summary>
    /// Category data for display
    /// </summary>
    public class CategoryDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }
}
