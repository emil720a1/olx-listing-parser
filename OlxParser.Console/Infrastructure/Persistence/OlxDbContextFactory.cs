using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using OlxParser.Console.Configuration;

namespace OlxParser.Console.Infrastructure.Persistence;

public sealed class OlxDbContextFactory
    : IDesignTimeDbContextFactory<OlxDbContext>
{
    public OlxDbContext CreateDbContext(string[] args)
    {
        var options = new ParserOptions();

        var databaseDirectory = Path.GetDirectoryName(options.DatabasePath);

        if (!string.IsNullOrWhiteSpace(databaseDirectory))
        {
            Directory.CreateDirectory(databaseDirectory);
        }

        var optionsBuilder =
            new DbContextOptionsBuilder<OlxDbContext>();

        optionsBuilder.UseSqlite(
            $"Data Source={options.DatabasePath}"
        );

        return new OlxDbContext(optionsBuilder.Options);
    }
}