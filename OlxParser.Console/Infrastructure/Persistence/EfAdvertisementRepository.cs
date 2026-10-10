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
        var existingAdvertisement =
            await _dbContext.Advertisements
                .FirstOrDefaultAsync(existing =>
                    existing.Id == advertisement.Id ||
                    existing.Url == advertisement.Url);

        if (existingAdvertisement is not null)
        {
            existingAdvertisement.Title = advertisement.Title;
            existingAdvertisement.Description =
                advertisement.Description ?? existingAdvertisement.Description;
            existingAdvertisement.AuthorName =
                advertisement.AuthorName ?? existingAdvertisement.AuthorName;
            existingAdvertisement.Phone =
                advertisement.Phone ?? existingAdvertisement.Phone;

            await _dbContext.SaveChangesAsync();

            return false;
        }

        _dbContext.Advertisements.Add(advertisement);

        await _dbContext.SaveChangesAsync();

        return true;
    }
}
