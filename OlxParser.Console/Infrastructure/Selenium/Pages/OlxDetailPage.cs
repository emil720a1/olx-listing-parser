using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using OlxParser.Console.Domain;

namespace OlxParser.Console.Infrastructure.Selenium.Pages;

public sealed class OlxDetailPage
{
    private readonly IWebDriver _driver;

    public OlxDetailPage(IWebDriver driver)
    {
        _driver = driver;
    }

    public Advertisement Parse(string url)
    {
        _driver.Navigate().GoToUrl(url);

        var wait = new WebDriverWait(
            _driver,
            TimeSpan.FromSeconds(15));

        wait.Until(driver => driver.FindElements(
            By.CssSelector("[data-testid='offer_title']")
            ).Count > 0
        );

        var title = _driver.FindElement(
            By.CssSelector("[data-testid='offer_title']")
        ).Text.Trim();

        var description = _driver.FindElements(
                By.CssSelector("[data-testid='ad-description-text']")
            )
            .FirstOrDefault()
            ?.Text
            .Trim();

        var authorName = _driver.FindElements(
                By.CssSelector("[data-testid='user-profile-user-name']")
            )
            .FirstOrDefault()
            ?.Text
            .Trim();

        return new Advertisement
        {
            Id = ExtractId(url),
            Title = title,
            Description = description,
            Url = url,
            AuthorName = authorName,
            Phone = null
        };
    }

    private static string ExtractId(string url)
    {
        var lastSegment = new Uri(url).Segments.Last().Trim('/');

        var extensionIndex = lastSegment.IndexOf(".html");

        if (extensionIndex >= 0)
        {
            lastSegment = lastSegment[..extensionIndex];
        }

        var separatorIndex = lastSegment.LastIndexOf("-");

        return separatorIndex >= 0
            ? lastSegment[(separatorIndex + 1)..]
            : lastSegment;

    }

}