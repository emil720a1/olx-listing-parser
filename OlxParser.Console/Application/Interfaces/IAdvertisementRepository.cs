using OlxParser.Console.Domain;

namespace OlxParser.Console.Application.Interfaces;

public interface IAdvertisementRepository
{
    Task<bool> SaveAsync(Advertisement advertisement);
}