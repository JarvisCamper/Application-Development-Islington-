namespace JournalApp.Models.Tag
{
    /// <summary>
    /// Tag data for display
    /// </summary>
    public class TagDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public bool IsCustom { get; set; }
    }
}
