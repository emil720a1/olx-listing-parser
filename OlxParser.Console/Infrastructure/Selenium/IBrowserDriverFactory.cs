using OpenQA.Selenium;

namespace OlxParser.Console.Infrastructure.Selenium;

public interface IBrowserDriverFactory
{
    IWebDriver CreateDriver();
}