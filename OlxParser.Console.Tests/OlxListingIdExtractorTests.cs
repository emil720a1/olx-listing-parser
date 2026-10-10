using OlxParser.Console.Application.Services;

namespace OlxParser_Console.Tests;

public sealed class OlxListingIdExtractorTests
{
    [Theory]
    [InlineData(
        "https://www.olx.ua/d/uk/obyavlenie/kurtka-ID11pUF8.html",
        "ID11pUF8")]
    [InlineData(
        "https://www.olx.ua/d/uk/obyavlenie/zimoviy-kombinezon-ID10D63r.html?search_reason=search%7Corganic",
        "ID10D63r")]
    public void Extract_ReturnsIdFromListingUrl(
        string url,
        string expectedId)
    {
        var result = OlxListingIdExtractor.Extract(url);

        Assert.Equal(expectedId, result);
    }

    [Fact]
    public void Extract_ReturnsLastSegmentWhenUrlHasNoHyphen()
    {
        var result = OlxListingIdExtractor.Extract(
            "https://www.olx.ua/ID123.html");

        Assert.Equal("ID123", result);
    }
}
