namespace JournalApp.Models.Analytics
{
    /// <summary>
    /// Word count trend data for charting
    /// </summary>
    public class WordCountTrendDto
    {
        public DateTime Date { get; set; }
        public int WordCount { get; set; }
        public int EntryCount { get; set; }
    }
}
