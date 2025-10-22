using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Sqlite; // Add this if needed

namespace Chirp.Razor.Data
{
    public class ChirpDBContext : DbContext
    {
        public DbSet<Cheep> Cheeps { get; set; }
        public DbSet<Author> Authors { get; set; }
        public string DbPath { get; }

        public ChirpDBContext(string? dbPath = null)
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

    public class Author
    {
        public int AuthorId { get; set; }                 // PK from dump
        public string Name { get; set; } = null!;
        public string Email { get; set; } = null!;
        public List<Cheep> Cheeps { get; set; } = new();
    }

    public class Cheep
    {
        public int CheepId { get; set; }                  // PK from dump
        public int AuthorId { get; set; }                 // FK -> Author.AuthorId
        public Author Author { get; set; } = null!;
        public string Text { get; set; } = null!;
        public DateTime TimeStamp { get; set; }
    }
}

