using OlxParser.Console.Infrastructure.Selenium;
using OlxParser.Console.Infrastructure.Selenium.Pages;

var driverFactory = new ChromeDriverFactory();

using var driver = driverFactory.CreateDriver();

driver.Navigate().GoToUrl(
    "https://www.olx.ua/uk/detskiy-mir/detskaya-odezhda/"
);

var listingPage = new OlxListingPage(driver);

var listingUrls = listingPage.GetListingUrls();

Console.WriteLine($"Found listings: {listingUrls.Count}");

var firstUrl = listingUrls.First();

var detailPage = new OlxDetailPage(driver);

var advertisement = detailPage.Parse(firstUrl);

Console.WriteLine($"Id: {advertisement.Id}");
Console.WriteLine($"Title: {advertisement.Title}");
Console.WriteLine($"Description: {advertisement.Description}");
Console.WriteLine($"Url: {advertisement.Url}");
Console.WriteLine($"Author: {advertisement.AuthorName}");
Console.WriteLine($"Phone: {advertisement.Phone}");