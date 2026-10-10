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

        string? description = null;

        try
        {
            var descriptionSelectors = new[]
            {
                "[data-testid='ad_description']",
                "[data-testid='ad-description-section']",
                "[data-testid='ad-description-text']",
                "[data-testid='textContainer']"
            };

            wait.Until(driver => descriptionSelectors.Any(selector =>
                driver.FindElements(By.CssSelector(selector))
                    .Any(element => !string.IsNullOrWhiteSpace(GetElementText(element)))));

            foreach (var selector in descriptionSelectors)
            {
                var element = _driver.FindElements(By.CssSelector(selector))
                    .FirstOrDefault(item =>
                        !string.IsNullOrWhiteSpace(GetElementText(item)));

                if (element is null)
                {
                    continue;
                }

                description = GetElementText(element)
                    .Replace("ОПИС", string.Empty)
                    .Replace("Докладніше", string.Empty)
                    .Trim();

                global::System.Console.WriteLine(
                    $"Description extracted: {description.Length} chars");

                break;
            }
        }
        catch (WebDriverTimeoutException)
        {
            global::System.Console.WriteLine(
                "Description was not available.");
        }

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

    private string GetElementText(IWebElement element)
    {
        var text = ((IJavaScriptExecutor)_driver).ExecuteScript(
            "return arguments[0].innerText || arguments[0].textContent || '';",
            element) as string;

        return text?.Trim() ?? string.Empty;
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
