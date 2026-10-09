using OpenQA.Selenium;

namespace OlxParser.Console.Infrastructure.Selenium.Pages;

public sealed class OlxListingPage
{
    private const string ListingLinkSelector =
        "a[href*='/d/uk/obyavlenie/']";

    private readonly IWebDriver _driver;

    public OlxListingPage(IWebDriver driver)
    {
        _driver = driver;
    }

    public IReadOnlyCollection<string> GetListingUrls()
    {
        var listingElements = _driver.FindElements(
            By.CssSelector(ListingLinkSelector)
            );

        return listingElements
            .Select(element => element.GetAttribute("href"))
            .Where(url => !string.IsNullOrWhiteSpace(url))
            .Distinct()
            .ToArray()!;
    }
}