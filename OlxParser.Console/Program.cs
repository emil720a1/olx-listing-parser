using OlxParser.Console.Infrastructure.Selenium;
using OlxParser.Console.Infrastructure.Selenium.Pages;
using Microsoft.EntityFrameworkCore;
using OlxParser.Console.Infrastructure.Persistence;

var driverFactory = new ChromeDriverFactory();

using var driver = driverFactory.CreateDriver();

var dbOptions = new DbContextOptionsBuilder<OlxDbContext>()
    .UseSqlite("Data Source=OlxParser.Console/olx_ads.sqlite3")
    .Options;

await using var dbContext = new OlxDbContext(dbOptions);

await dbContext.Database.MigrateAsync();

var repository = new EfAdvertisementRepository(dbContext);

driver.Navigate().GoToUrl(
    "https://www.olx.ua/uk/detskiy-mir/detskaya-odezhda/"
);

var listingPage = new OlxListingPage(driver);

var listingUrls = listingPage.GetListingUrls();

Console.WriteLine($"Found listings: {listingUrls.Count}");

var firstUrl = listingUrls.First();

var detailPage = new OlxDetailPage(driver);

var advertisement = detailPage.Parse(firstUrl);

var saved = await repository.SaveAsync(advertisement);

Console.WriteLine(
    saved
        ? "Advertisement saved to database"
        : "Advertisement already exists"
);

Console.WriteLine($"Id: {advertisement.Id}");
Console.WriteLine($"Title: {advertisement.Title}");
Console.WriteLine($"Description: {advertisement.Description}");
Console.WriteLine($"Url: {advertisement.Url}");
Console.WriteLine($"Author: {advertisement.AuthorName}");
Console.WriteLine($"Phone: {advertisement.Phone}");