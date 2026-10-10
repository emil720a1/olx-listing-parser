using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace OlxParser.Console.Infrastructure.Selenium;

public sealed class BrowserDriverFactory : IBrowserDriverFactory
{
    public IWebDriver CreateDriver()
    {
        var options = new ChromeOptions();

        var remoteUrl =
            Environment.GetEnvironmentVariable("SELENIUM_REMOTE_URL");

        if (!string.IsNullOrWhiteSpace(remoteUrl))
        {
            options.AddArgument("--headless=new");
            options.AddArgument("--no-sandbox");
            options.AddArgument("--disable-dev-shm-usage");
            options.PageLoadStrategy = PageLoadStrategy.Eager;

            var remoteDriver = new OpenQA.Selenium.Remote.RemoteWebDriver(
                new Uri(remoteUrl),
                options
            );

            remoteDriver.Manage().Timeouts().PageLoad =
                TimeSpan.FromSeconds(30);

            return remoteDriver;
        }

        var isContainer =
            string.Equals(
                Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER"),
                "true",
                StringComparison.OrdinalIgnoreCase);

        if (isContainer)
        {
            options.AddArgument("--headless");
            options.AddArgument("--no-sandbox");
            options.AddArgument("--disable-dev-shm-usage");
        }
        else
        {
            options.AddArgument("--start-maximized");
        }

        options.PageLoadStrategy = PageLoadStrategy.Eager;

        var driver = new ChromeDriver(options);

        driver.Manage().Timeouts().PageLoad =
            TimeSpan.FromSeconds(30);

        return driver;
    }
}
