using OlxParser.Console.Infrastructure.Selenium;
using OlxParser.Console.Infrastructure.Selenium.Pages;
using Microsoft.EntityFrameworkCore;
using OlxParser.Console.Configuration;
using OlxParser.Console.Infrastructure.Persistence;

var options = new ParserOptions();

var databaseDirectory = Path.GetDirectoryName(options.DatabasePath);

if (!string.IsNullOrWhiteSpace(databaseDirectory))
{
    Directory.CreateDirectory(databaseDirectory);
}

var driverFactory = new BrowserDriverFactory();

using var driver = driverFactory.CreateDriver();

var dbOptions = new DbContextOptionsBuilder<OlxDbContext>()
    .UseSqlite($"Data Source={options.DatabasePath}")
    .Options;

await using var dbContext = new OlxDbContext(dbOptions);

await dbContext.Database.MigrateAsync();

var repository = new EfAdvertisementRepository(dbContext);

var listingPage = new OlxListingPage(driver);

var listingUrls = new HashSet<string>();

for (var pageNumber = 1; pageNumber <= options.PagesToParse; pageNumber++)
{
    var pageUrl = pageNumber == 1
        ? options.CategoryUrl
        : $"{options.CategoryUrl}?page={pageNumber}";

    Console.WriteLine($"Opening listing page {pageNumber}: {pageUrl}");

    driver.Navigate().GoToUrl(pageUrl);

    var pageListingUrls = listingPage.GetListingUrls();

    foreach (var listingUrl in pageListingUrls)
    {
        listingUrls.Add(listingUrl);
    }

    Console.WriteLine(
        $"Page {pageNumber}: {pageListingUrls.Count} links"
        );
}

Console.WriteLine($"Found unique listings: {listingUrls.Count}");

var detailPage = new OlxDetailPage(driver);

foreach (var url in listingUrls)
{
    try
    {
        Console.WriteLine($"Processing: {url}");

        var advertisement = detailPage.Parse(url);

        var saved = await repository.SaveAsync(advertisement);

        Console.WriteLine(
            saved
                ? $"Saved: {advertisement.Id}"
                : $"Already exists: {advertisement.Id}"
        );
    }
    catch (Exception exception)
    {
        Console.WriteLine(
            $"Failed to process {url}: {exception.Message}"
        );
    }
}