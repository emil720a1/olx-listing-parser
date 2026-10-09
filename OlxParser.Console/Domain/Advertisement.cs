namespace OlxParser.Console.Domain;

public sealed class Advertisement
{
    public required string Id { get; init; }

    public required string Title { get; init; }

    public string? Description { get; init; }

    public required string Url { get; init; }

    public string? AuthorName { get; init; }

    public string? Phone { get; init; }
}