using Microsoft.EntityFrameworkCore;
using OS.Data.Models;

namespace OS.Data.Context;

public class OsDbContext(DbContextOptions<OsDbContext> options) : DbContext(options)
{
    public DbSet<Album> Albums { get; set; }
    public DbSet<Artist> Artists { get; set; }
    public DbSet<Track> Tracks { get; set; }
    public DbSet<Playlist> Playlists { get; set; }
    public DbSet<StoredEntity> StoredEntities { get; set; }

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

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        if (ChangeTracker.HasChanges())
        {
            var entries = ChangeTracker.Entries().Where(e => e.State == EntityState.Added || e.State == EntityState.Modified);
            foreach (var entry in entries)
            {
                if (entry.Entity is BaseModel model)
                {
                    if (entry.State == EntityState.Added)
                    {
                        model.CreatedAt = DateTime.UtcNow;
                    }
                    model.UpdatedAt = DateTime.UtcNow;
                }
            }
        }
        return base.SaveChangesAsync(cancellationToken);
    }
}