using OlxParser.Console.Infrastructure.Selenium;

var driverFactory = new ChromeDriverFactory();

using var driver = driverFactory.CreateDriver();

driver.Navigate().GoToUrl(
    "https://www.olx.ua/uk/detskiy-mir/detskaya-odezhda/"
    );

Console.WriteLine($"Title: {driver.Title}");
Console.WriteLine($"URL: {driver.Url}");