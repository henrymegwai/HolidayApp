using HolidayApp.Application.Common.Models.Dtos;
using HolidayApp.Domain.Entities;

namespace HolidayApp.Application.Common.Interfaces;

/// <summary>
///  Repository interface for managing holiday-related data operations.
/// </summary>
public interface IHolidayRepository
{
    /// <summary>
    ///  Gets the last celebrated holidays for a given country code.
    /// </summary>
    /// <param name="countryCode"></param>
    /// <param name="count"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<List<LastCelebratedHolidayDto>> GetLastCelebratedHolidaysAsync(string countryCode,
        int count,
        CancellationToken cancellationToken);
    
    /// <summary>
    ///  Gets the count of holidays falling on each weekday for the specified year and country codes.
    /// </summary>
    /// <param name="year"></param>
    /// <param name="countryCodes"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<List<CountryHolidayCountDto>> GetWeekdayHolidayCountsAsync(int year,
        string[] countryCodes,
        CancellationToken cancellationToken);
    
    /// <summary>
    ///  Gets the list of common holidays between two countries for a specified year.
    /// </summary>
    /// <param name="year"></param>
    /// <param name="countryCode1"></param>
    /// <param name="countryCode2"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<List<CommonHolidayDto>> GetCommonHolidaysAsync(int year,
        string countryCode1,
        string countryCode2,
        CancellationToken cancellationToken);
    
    /// <summary>
    ///  Inserts a list of holidays into the database in bulk.
    /// </summary>
    /// <param name="holidays"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task BulkInsertHolidaysAsync(List<Holiday> holidays, CancellationToken cancellationToken);
    
    /// <summary>
    ///  Checks if holidays exist for a given country code and year.
    /// </summary>
    /// <param name="countryCode"></param>
    /// <param name="year"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<bool> HolidaysExistForYearAsync(string countryCode, int year, CancellationToken cancellationToken);
}