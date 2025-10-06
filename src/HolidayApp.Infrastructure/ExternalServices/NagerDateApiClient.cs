using System.Text.Json;
using HolidayApp.Application.Common.Interfaces;
using HolidayApp.Application.Common.Models.Dtos;
using HolidayApp.Infrastructure.ExternalServices.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace HolidayApp.Infrastructure.ExternalServices;

public class NagerDateApiClient(
    HttpClient httpClient,
    IOptions<NagerDateApiConfiguration> nagerDateApiConfigurationOptions,
    ILogger<NagerDateApiClient> logger) : INagerDateApiClient
{
    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };
    
    private readonly string _serviceUrl = nagerDateApiConfigurationOptions.Value.ServiceUrl;

    public async Task<List<CountryInfoDto>> GetAvailableCountriesAsync()
    {
        logger.LogInformation("Fetching available countries from Nager.Date Api");

        try
        {
            var response = await httpClient.GetAsync($"{_serviceUrl}/AvailableCountries");
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var countries = JsonSerializer.Deserialize<List<CountryInfoDto>>(content, _jsonOptions)
                            ?? [];
            
            logger.LogInformation("Successfully fetched {Count} countries", countries.Count);
            return countries;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to fetch countries from API");
            throw;
        }
    }

    public async Task<List<PublicHolidayDto>> GetPublicHolidaysAsync(int year, string countryCode)
    {
        logger.LogInformation("Fetching holidays for {CountryCode} in {Year}", countryCode, year);

        try
        {
            var response = await httpClient.GetAsync($"{_serviceUrl}/PublicHolidays/{year}/{countryCode}");
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var holidays = JsonSerializer.Deserialize<List<PublicHolidayDto>>(content, _jsonOptions)
                           ?? new List<PublicHolidayDto>();

            logger.LogInformation("Successfully fetched {Count} holidays for {CountryCode}",
                holidays.Count, countryCode);
            return holidays;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to fetch holidays for {CountryCode} in {Year}",
                countryCode, year);
            throw;
        }
    }
}