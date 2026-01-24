using JournalApp.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace JournalApp.Data
{
    public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
    {
        public AppDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
            // Use a dummy path for migration generation, implementation will override at runtime
            optionsBuilder.UseSqlite("Data Source=journal.db");

            return new AppDbContext(optionsBuilder.Options);
        }
    }
}
