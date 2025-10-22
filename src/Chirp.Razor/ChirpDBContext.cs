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

        public ChirpDBContext(DbContextOptions options) : base(options)
        {
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

