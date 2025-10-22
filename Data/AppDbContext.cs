using Microsoft.EntityFrameworkCore;
using MusicPlayerWeb.Models;

namespace MusicPlayerWeb.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<User> Users => Set<User>();
        public DbSet<Track> Tracks => Set<Track>();

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder b)
        {
            // Users
            b.Entity<User>(e =>
            {
                e.HasKey(x => x.Id);
                e.Property(x => x.Username).IsRequired().HasMaxLength(100);
                e.Property(x => x.PasswordHash).IsRequired();
                e.HasIndex(x => x.Username).IsUnique();
            });

            // Tracks
            b.Entity<Track>(e =>
            {
                e.HasKey(x => x.Id);
                e.Property(x => x.Title).IsRequired().HasMaxLength(256);
                e.Property(x => x.FileName).IsRequired().HasMaxLength(256);
                e.Property(x => x.RelativeUrl).IsRequired().HasMaxLength(512);
                e.Property(x => x.OwnerUsername).HasMaxLength(100);
                e.HasIndex(x => new { x.OwnerUsername, x.AddedAt });
            });
        }
    }
}
