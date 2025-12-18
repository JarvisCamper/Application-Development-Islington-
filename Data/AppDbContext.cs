using JournalApp.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Maui.Storage;
using System.Collections.Generic;
using System.IO;

namespace JournalApp.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<JournalEntry> JournalEntries { get; set; } = default!;

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            string dbPath = Path.Combine(
                FileSystem.AppDataDirectory,
                "journal.db");
            options.UseSqlite($"Filename={dbPath}");
        }
    }
}