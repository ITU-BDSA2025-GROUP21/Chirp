using Microsoft.EntityFrameworkCore;
using Chirp.Infrastructure.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace Chirp.Infrastructure.Data
{
    public class ChirpDBContext : IdentityDbContext<Author>
    {
        public DbSet<Cheep> Cheeps { get; set; }
        public DbSet<Author> Authors { get; set; }

        public ChirpDBContext(DbContextOptions<ChirpDBContext> options) 
            : base(options)
        {
        }
    }
}

