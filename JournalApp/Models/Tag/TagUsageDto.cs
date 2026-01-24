namespace JournalApp.Models.Tag
{
    /// <summary>
    /// Tag with usage statistics
    /// </summary>
    public class TagUsageDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int UsageCount { get; set; }
    }
}
