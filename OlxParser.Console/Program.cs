using OlxParser.Console.Infrastructure.Selenium;
using OlxParser.Console.Infrastructure.Selenium.Pages;
using Microsoft.EntityFrameworkCore;
using OlxParser.Console.Configuration;
using OlxParser.Console.Infrastructure.Persistence;
using OpenQA.Selenium;

var options = new ParserOptions();

var databaseDirectory = Path.GetDirectoryName(options.DatabasePath);

if (!string.IsNullOrWhiteSpace(databaseDirectory))
{
    Directory.CreateDirectory(databaseDirectory);
}

var driverFactory = new BrowserDriverFactory();
IWebDriver driver = driverFactory.CreateDriver();

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
var processedCount = 0;
var failedCount = 0;

void RestartDriver()
{
    DisposeDriver();
    driver = driverFactory.CreateDriver();
    detailPage = new OlxDetailPage(driver);
}

void DisposeDriver()
{
    try
    {
        driver.Quit();
    }
    catch (WebDriverException)
    {
        // The remote session may already be gone after a browser crash.
    }

    driver.Dispose();
}

try
{
    foreach (var url in listingUrls)
    {
        if (processedCount > 0 && processedCount % 50 == 0)
        {
            Console.WriteLine("Restarting browser session...");
            RestartDriver();
        }

        var processed = false;

        for (var attempt = 1; attempt <= 3 && !processed; attempt++)
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

                processed = true;
            }
            catch (WebDriverException exception) when (attempt < 3)
            {
                Console.WriteLine(
                    $"Browser session failed. Restarting and retrying: {exception.Message}"
                );

                RestartDriver();
            }
            catch (Exception exception)
            {
                Console.WriteLine(
                    $"Failed to process {url}: {exception.Message}"
                );

                failedCount++;
                processed = true;
            }
        }

        processedCount++;
    }
}
finally
{
    DisposeDriver();
}

Console.WriteLine(
    $"Processing completed. Total: {listingUrls.Count}; Failed: {failedCount}");

Environment.ExitCode = failedCount == 0 ? 0 : 1;
