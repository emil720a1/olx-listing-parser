using Microsoft.EntityFrameworkCore;
using OlxParser.Console.Domain;

namespace OlxParser.Console.Infrastructure.Persistence;

public sealed class OlxDbContext : DbContext
{
    public OlxDbContext(DbContextOptions<OlxDbContext> options)
        : base(options)
    {
    }

    public DbSet<Advertisement> Advertisements =>
        Set<Advertisement>();

    protected override void OnModelCreating(
        ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Advertisement>(entity =>
        {
            entity.ToTable("advertisements");

            entity.HasKey(advertisement => advertisement.Id);

            entity.Property(advertisement => advertisement.Id)
                .IsRequired();

            entity.Property(advertisement => advertisement.Title)
                .IsRequired();

            entity.Property(advertisement => advertisement.Url)
                .IsRequired();

            entity.HasIndex(advertisement => advertisement.Url)
                .IsUnique();
        });
    }
}