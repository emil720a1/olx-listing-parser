namespace OlxParser.Console.Application.Services;

public static class OlxListingIdExtractor
{
    public static string Extract(string url)
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
