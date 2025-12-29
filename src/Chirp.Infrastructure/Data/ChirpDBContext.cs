using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Chirp.Core.Models;

namespace Chirp.Core.Data
{
    /// <summary>
    /// Represents the primary database context for the Chirp application.
    /// 
    /// This DbContext is responsible for configuring and managing access to
    /// all persisted domain data, including authors, cheeps, likes, and
    /// follow relationships. It integrates ASP.NET Core Identity by inheriting
    /// from IdentityDbContext, enabling authentication and user-related data
    /// to coexist with application-specific entities.
    /// 
    /// The context is used at runtime by Entity Framework Core to:
    /// - Query and persist application data
    /// - Define entity relationships and composite keys
    /// - Enforce referential integrity and cascade delete behavior
    /// 
    /// </summary>
    public class ChirpDBContext : IdentityDbContext<Author>
    {
        public DbSet<Cheep> Cheeps { get; set; }
        public DbSet<Author> Authors { get; set; }
        public DbSet<UserFollow> UserFollows { get; set; }
        public DbSet<Like> Likes { get; set; } = null!;

        public ChirpDBContext(DbContextOptions<ChirpDBContext> options) 
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<UserFollow>()
                .HasKey(uf => new { uf.FollowerId, uf.FolloweeId });

            modelBuilder.Entity<Like>()
                .HasKey(l => new { l.authorId, l.CheepId });

            modelBuilder.Entity<Like>()
                .HasOne(l => l.Cheep)
                .WithMany(c => c.Likes) 
                .HasForeignKey(l => l.CheepId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Like>()
                .HasOne(l => l.Author)
                .WithMany() 
                .HasForeignKey(l => l.authorId)
                .OnDelete(DeleteBehavior.Cascade);


            modelBuilder.Entity<UserFollow>()
                .HasOne(uf => uf.Follower)
                .WithMany()
                .HasForeignKey(uf => uf.FollowerId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<UserFollow>()
                .HasOne(uf => uf.Followee)
                .WithMany()
                .HasForeignKey(uf => uf.FolloweeId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}

