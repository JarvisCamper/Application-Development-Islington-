using JournalApp.Data;
using JournalApp.Models;
using Microsoft.EntityFrameworkCore;

namespace JournalApp.Services
{
    public class JournalService
    {
        private readonly AppDbContext _db;

        public JournalService(AppDbContext db)
        {
            _db = db;
        }

        public async Task<List<JournalEntry>> GetAllEntriesAsync()
        {
            return await _db.JournalEntries
                            .OrderByDescending(e => e.EntryDate)
                            .ToListAsync();
        }

        public async Task AddEntryAsync(JournalEntry entry)
        {
            _db.JournalEntries.Add(entry);
            await _db.SaveChangesAsync();
        }
    }
}
