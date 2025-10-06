using HolidayApp.Application.Common.Models.Dtos;

namespace HolidayApp.Application.Common.Interfaces;

/// <summary>
///  Client interface for interacting with the Nager.Date Api to fetch country and public holiday information.
/// </summary>
public interface INagerDateApiClient
{
    /// <summary>
    ///  Fetches a list of available countries from the Nager.Date Api.
    /// </summary>
    /// <returns></returns>
    Task<List<CountryInfoDto>> GetAvailableCountriesAsync();
    
    /// <summary>
    ///  Fetches public holidays for a specified year and country code from the Nager.Date Api.
    /// </summary>
    /// <param name="year"></param>
    /// <param name="countryCode"></param>
    /// <returns></returns>
    Task<List<PublicHolidayDto>> GetPublicHolidaysAsync(int year, string countryCode);
}