using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace OlxParser.Console.Infrastructure.Selenium;

public sealed class BrowserDriverFactory : IBrowserDriverFactory
{
    public IWebDriver CreateDriver()
    {
        var options = new ChromeOptions();

        options.AddArgument("--start-maximized");
        options.PageLoadStrategy = PageLoadStrategy.Eager;

        var driver = new ChromeDriver(options);

        driver.Manage().Timeouts().PageLoad =
            TimeSpan.FromSeconds(30);

        return driver;
    }
}
