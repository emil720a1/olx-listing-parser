using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using OlxParser.Console.Domain;
using OlxParser.Console.Infrastructure.Persistence;

namespace OlxParser_Console.Tests;

public sealed class EfAdvertisementRepositoryTests : IDisposable
{
    private readonly SqliteConnection _connection;
    private readonly OlxDbContext _dbContext;

    public EfAdvertisementRepositoryTests()
    {
        _connection = new SqliteConnection("Data Source=:memory:");
        _connection.Open();

        var options = new DbContextOptionsBuilder<OlxDbContext>()
            .UseSqlite(_connection)
            .Options;

        _dbContext = new OlxDbContext(options);
        _dbContext.Database.EnsureCreated();
    }

    [Fact]
    public async Task SaveAsync_AddsNewAdvertisement()
    {
        var repository = new EfAdvertisementRepository(_dbContext);

        var result = await repository.SaveAsync(CreateAdvertisement());

        Assert.True(result);
        Assert.Equal(1, await _dbContext.Advertisements.CountAsync());
    }

    [Fact]
    public async Task SaveAsync_DoesNotCreateDuplicateById()
    {
        var repository = new EfAdvertisementRepository(_dbContext);

        await repository.SaveAsync(CreateAdvertisement());
        var result = await repository.SaveAsync(CreateAdvertisement());

        Assert.False(result);
        Assert.Equal(1, await _dbContext.Advertisements.CountAsync());
    }

    [Fact]
    public async Task SaveAsync_DoesNotCreateDuplicateByUrl()
    {
        var repository = new EfAdvertisementRepository(_dbContext);

        await repository.SaveAsync(CreateAdvertisement());

        var advertisementWithSameUrl = CreateAdvertisement("ID456");

        var result = await repository.SaveAsync(advertisementWithSameUrl);

        Assert.False(result);
        Assert.Equal(1, await _dbContext.Advertisements.CountAsync());
    }

    [Fact]
    public async Task SaveAsync_UpdatesExistingAdvertisement()
    {
        var repository = new EfAdvertisementRepository(_dbContext);

        await repository.SaveAsync(CreateAdvertisement());

        var updated = CreateAdvertisement();
        updated.Title = "Updated title";
        await repository.SaveAsync(updated);

        var stored = await _dbContext.Advertisements.SingleAsync();

        Assert.Equal("Updated title", stored.Title);
    }

    [Fact]
    public async Task SaveAsync_PreservesExistingOptionalDataWhenNewValuesAreNull()
    {
        var repository = new EfAdvertisementRepository(_dbContext);
        var original = CreateAdvertisement();

        await repository.SaveAsync(original);

        var updated = CreateAdvertisement();
        updated.Description = null;
        updated.AuthorName = null;
        updated.Phone = null;
        await repository.SaveAsync(updated);

        var stored = await _dbContext.Advertisements.SingleAsync();

        Assert.Equal(original.Description, stored.Description);
        Assert.Equal(original.AuthorName, stored.AuthorName);
        Assert.Equal(original.Phone, stored.Phone);
    }

    public void Dispose()
    {
        _dbContext.Dispose();
        _connection.Dispose();
    }

    private static Advertisement CreateAdvertisement(
        string id = "ID123")
    {
        return new Advertisement
        {
            Id = id,
            Title = "Test advertisement",
            Description = "Original description",
            Url = "https://www.olx.ua/d/uk/obyavlenie/test-ID123.html",
            AuthorName = "Test seller",
            Phone = "+380000000000"
        };
    }
}
