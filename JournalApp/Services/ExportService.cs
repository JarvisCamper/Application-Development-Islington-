using JournalApp.Data;
using JournalApp.Models.Common;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Colors = QuestPDF.Helpers.Colors;

namespace JournalApp.Services
{
    public class ExportService : IExportService
    {
        private readonly AppDbContext _context;

        public ExportService(AppDbContext context)
        {
            _context = context;
            
            // Configure QuestPDF license
            QuestPDF.Settings.License = LicenseType.Community;
        }

        public async Task<byte[]> ExportToPdfAsync(DateRangeDto dateRange)
        {
            var query = _context.JournalEntries
                .Include(e => e.PrimaryMood)
                .Include(e => e.SecondaryMood1)
                .Include(e => e.SecondaryMood2)
                .Include(e => e.Category)
                .Include(e => e.Tags)
                .AsQueryable();

             if (dateRange?.StartDate.HasValue == true)
            {
                query = query.Where(e => e.Date >= dateRange.StartDate.Value);
            }

            if (dateRange?.EndDate.HasValue == true)
            {
                query = query.Where(e => e.Date <= dateRange.EndDate.Value);
            }

            var entries = await query.OrderBy(e => e.Date).ToListAsync();

            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(2, Unit.Centimetre);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(11));

                    page.Header()
                        .Text($"Journal Entries Export")
                        .FontSize(20)
                        .SemiBold()
                        .FontColor(Colors.Blue.Medium);

                    page.Content()
                        .PaddingVertical(1, Unit.Centimetre)
                        .Column(column =>
                        {
                            column.Spacing(15);

                            column.Item().Text(text =>
                            {
                                text.Span("Export Period: ").SemiBold();
                                text.Span($"{(dateRange?.StartDate?.ToString("MMM d, yyyy") ?? "Beginning")} - {(dateRange?.EndDate?.ToString("MMM d, yyyy") ?? "End")}");
                            });

                            column.Item().Text(text =>
                            {
                                text.Span("Total Entries: ").SemiBold();
                                text.Span($"{entries.Count}");
                            });

                            column.Item().LineHorizontal(1).LineColor(Colors.Grey.Lighten2);

                            foreach (var entry in entries)
                            {
                                column.Item().Element(container =>
                                {
                                    container.Column(entryColumn =>
                                    {
                                        // Date and Title
                                        entryColumn.Item().Text(entry.Date.ToString("MMMM d, yyyy"))
                                            .FontSize(14)
                                            .SemiBold()
                                            .FontColor(Colors.Blue.Darken1);

                                        entryColumn.Item().PaddingTop(5).Text(entry.Title)
                                            .FontSize(13)
                                            .SemiBold();

                                        entryColumn.Item().PaddingTop(5).Text(text =>
                                        {
                                            text.Span("Mood: ").FontSize(10).FontColor(Colors.Grey.Darken1);
                                            text.Span($"{entry.PrimaryMood.Icon} {entry.PrimaryMood.Name}").FontSize(10);
                                            
                                            if (entry.SecondaryMood1 != null)
                                            {
                                                text.Span($", {entry.SecondaryMood1.Icon} {entry.SecondaryMood1.Name}").FontSize(10);
                                            }
                                            
                                            if (entry.SecondaryMood2 != null)
                                            {
                                                text.Span($", {entry.SecondaryMood2.Icon} {entry.SecondaryMood2.Name}").FontSize(10);
                                            }
                                        });

                                        if (entry.Category != null || entry.Tags.Any())
                                        {
                                            entryColumn.Item().Text(text =>
                                            {
                                                if (entry.Category != null)
                                                {
                                                    text.Span("Category: ").FontSize(10).FontColor(Colors.Grey.Darken1);
                                                    text.Span($"{entry.Category.Name}  ").FontSize(10);
                                                }

                                                if (entry.Tags.Any())
                                                {
                                                    text.Span("Tags: ").FontSize(10).FontColor(Colors.Grey.Darken1);
                                                    text.Span(string.Join(", ", entry.Tags.Select(t => t.Name))).FontSize(10);
                                                }
                                            });
                                        }

                                        entryColumn.Item().PaddingTop(8).Text(entry.Content)
                                            .FontSize(11)
                                            .LineHeight(1.5f);

                                        // Separator
                                        entryColumn.Item().PaddingTop(10).LineHorizontal(1).LineColor(Colors.Grey.Lighten3);
                                    });
                                });

                                column.Item().PageBreak();
                            }
                        });

                    page.Footer()
                        .AlignCenter()
                        .Text(x =>
                        {
                            x.Span("Page ");
                            x.CurrentPageNumber();
                            x.Span(" of ");
                            x.TotalPages();
                        });
                });
            });

            return document.GeneratePdf();
        }
    }
}
