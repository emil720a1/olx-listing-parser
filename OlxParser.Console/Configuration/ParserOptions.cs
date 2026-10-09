namespace OlxParser.Console.Configuration;

public sealed class ParserOptions
{
    public string CategoryUrl { get; init; } =
        "https://www.olx.ua/uk/detskiy-mir/detskaya-odezhda/";

    public string DatabasePath { get; init; } =
        "data/olx_ads.sqlite3";

    public int PagesToParse { get; init; } = 10;
}