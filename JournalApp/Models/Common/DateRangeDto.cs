namespace JournalApp.Models.Common
{
    /// <summary>
    /// Date range for filtering
    /// </summary>
    public class DateRangeDto
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}
