using HolidayApp.Domain.Entities;

namespace HolidayApp.Application.Common.Interfaces;
/// <summary>
///  Repository interface for managing Country entities.
/// </summary>
public interface ICountryRepository
{
    /// <summary>
    ///  Fetches a Country entity based on its country code.
    /// </summary>
    /// <param name="countryCode"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<Country?> GetByCodeAsync(string countryCode, CancellationToken cancellationToken);
    
    /// <summary>
    ///  Fetches all Country entities.
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<List<Country>> GetAllAsync(CancellationToken cancellationToken);
    
    /// <summary>
    ///  Checks if a Country with the specified country code exists.
    /// </summary>
    /// <param name="countryCode"></param>
    /// <returns></returns>
    Task<bool> CountryExistsAsync(string countryCode);
    
    /// <summary>
    ///  Adds a list of Country entities to the repository.
    /// </summary>
    /// <param name="countries"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task AddRangeAsync(List<Country> countries, CancellationToken cancellationToken);
}