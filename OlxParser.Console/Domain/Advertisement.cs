namespace OlxParser.Console.Domain;

public sealed class Advertisement
{
    public required string Id { get; init; }

    public required string Title { get; set; }

    public string? Description { get; set; }

    public required string Url { get; init; }

    public string? AuthorName { get; set; }

    public string? Phone { get; set; }
}