using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Chirp.Core.Models;

namespace Chirp.Core.Data
{
    public class ChirpDBContext : IdentityDbContext<Author>
    {
        public DbSet<Cheep> Cheeps { get; set; }
        public DbSet<Author> Authors { get; set; }
        public DbSet<UserFollow> UserFollows { get; set; }
        public DbSet<Likes> Likes { get; set; } = null!;

        public ChirpDBContext(DbContextOptions<ChirpDBContext> options) 
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Likes>()
                .HasKey(l => new { l.authorId, l.CheepId });

            modelBuilder.Entity<UserFollow>()
                .HasKey(uf => new { uf.FollowerId, uf.FolloweeId });

            modelBuilder.Entity<Likes>()
                .HasOne<Cheep>()
                .WithMany()
                .HasForeignKey(l => l.CheepId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Likes>()
                .HasOne<Author>()
                .WithMany()
                .HasForeignKey(l => l.authorId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<UserFollow>()
                .HasOne(uf => uf.Follower)
                .WithMany()
                .HasForeignKey(uf => uf.FollowerId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<UserFollow>()
                .HasOne(uf => uf.Followee)
                .WithMany()
                .HasForeignKey(uf => uf.FolloweeId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}

