using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Sqlite; // Add this if needed

namespace Chirp.Razor.Data
{
    public class CheepRepository : DbContext
    {
        public DbSet<Cheep> cheeps { get; set; }
        public DbSet<Author> authors { get; set; }
        public string DbPath { get; }

        public CheepRepository(string? dbPath = null)
        {
            var envPath = Environment.GetEnvironmentVariable("CHIRPDBPATH");
            var tempPath = Path.Combine(Path.GetTempPath(), "chirp.db");

            // choose base path: parameter > env > temp
            var chosen = !string.IsNullOrWhiteSpace(dbPath) ? dbPath
                : !string.IsNullOrWhiteSpace(envPath) ? envPath
                : tempPath;

            // ensure it ends with .db and has a directory
            if (!chosen.EndsWith(".db", StringComparison.OrdinalIgnoreCase))
            {
                var directory = Path.GetDirectoryName(chosen);
                chosen = string.IsNullOrEmpty(directory)
                    ? tempPath
                    : Path.Combine(directory, "chirp.db");
            }

            DbPath = chosen;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite($"Data Source={DbPath}");

        }

    }

    public class Author()
    {
    public string email { get; set; }
    public string name { get; set; }
    public List<Cheep> cheeps { get; set; }

    }

    public class Cheep()
    {
    public string text { get; set; }
    public DateTime timestamp { get; set; }
    public Author author { get; set; }
    }
}