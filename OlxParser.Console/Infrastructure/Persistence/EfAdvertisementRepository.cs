using Microsoft.EntityFrameworkCore;
using OlxParser.Console.Application.Interfaces;
using OlxParser.Console.Domain;

namespace OlxParser.Console.Infrastructure.Persistence;

public sealed class EfAdvertisementRepository
    : IAdvertisementRepository
{
    private readonly OlxDbContext _dbContext;

    public EfAdvertisementRepository(OlxDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<bool> SaveAsync(
        Advertisement advertisement)
    {
        var alreadyExists = await _dbContext.Advertisements
            .AnyAsync(existing =>
                existing.Id == advertisement.Id ||
                existing.Url == advertisement.Url);

        if (alreadyExists)
        {
            return false;
        }

        _dbContext.Advertisements.Add(advertisement);

        await _dbContext.SaveChangesAsync();

        return true;
    }
}
