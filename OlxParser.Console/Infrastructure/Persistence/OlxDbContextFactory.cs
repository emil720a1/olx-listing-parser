using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace OlxParser.Console.Infrastructure.Persistence;

public sealed class OlxDbContextFactory
    : IDesignTimeDbContextFactory<OlxDbContext>
{
    public OlxDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder =
            new DbContextOptionsBuilder<OlxDbContext>();

        optionsBuilder.UseSqlite(
            "Data Source=olx_ads.sqlite3"
            );

        return new OlxDbContext(optionsBuilder.Options);
    }
}