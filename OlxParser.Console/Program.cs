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

foreach (var listingUrl in listingUrls)
{
    Console.WriteLine(listingUrl);
}