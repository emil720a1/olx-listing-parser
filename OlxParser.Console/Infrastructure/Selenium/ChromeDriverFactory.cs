using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace OlxParser.Console.Infrastructure.Selenium;

public class ChromeDriverFactory : IBrowseDriverFactory
{
    public IWebDriver CreateDriver()
    {
        var options = new ChromeOptions();
        options.AddArgument("--start-maximized");

        return new ChromeDriver(options);
    }
}