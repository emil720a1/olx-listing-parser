using OpenQA.Selenium;

namespace OlxParser.Console.Infrastructure.Selenium;

public interface IBrowseDriverFactory
{
    IWebDriver CreateDriver();
}