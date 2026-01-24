using JournalApp.Models.Common;

namespace JournalApp.Services
{
    /// <summary>
    /// Export service for PDF generation
    /// </summary>
    public interface IExportService
    {
        Task<byte[]> ExportToPdfAsync(DateRangeDto dateRange);
    }
}
