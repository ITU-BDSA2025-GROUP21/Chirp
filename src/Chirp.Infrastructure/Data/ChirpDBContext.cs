using Microsoft.EntityFrameworkCore;
using Chirp.Infrastructure.Models;

namespace Chirp.Infrastructure.Data
{
    public class ChirpDBContext : DbContext
    {

        public DbSet<Cheep> Cheeps { get; set; }
        public DbSet<Author> Authors { get; set; }

        public ChirpDBContext(DbContextOptions options) : base(options)
        {
        }

    }
}

