using Microsoft.EntityFrameworkCore;
using OS.Data.Models;

namespace OS.Data.Context;

public class OsDbContext(DbContextOptions<OsDbContext> options) : DbContext(options)
{
    public DbSet<Album> Albums { get; set; }
    public DbSet<Artist> Artists { get; set; }
    public DbSet<Track> Tracks { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Artist>().HasData(
            new Artist
            {
                Id = Guid.Parse("00000000-0000-0000-0000-000000000001"),
                Name = "Unknown"
            }
        );
        base.OnModelCreating(modelBuilder);
    }
}