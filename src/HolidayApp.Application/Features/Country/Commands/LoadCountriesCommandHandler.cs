using HolidayApp.Application.Common.Abstractions;
using HolidayApp.Application.Common.Decorators;
using HolidayApp.Application.Common.Interfaces;
using HolidayApp.Application.Common.Mapper;
using HolidayApp.Application.Common.Models;
using Microsoft.Extensions.Logging;

namespace HolidayApp.Application.Features.Country.Commands;

public class LoadCountriesCommandHandler(
    ICountryRepository countryRepository,
    INagerDateApiClient apiClient,
    ICacheReadWriteDecorator<LoadCountriesCommand, LoadCountriesResult> cacheDecorator,
    ILogger<LoadCountriesCommandHandler> logger)
    : BaseDecoratedCommandHandler<LoadCountriesCommand, LoadCountriesResult>(logger)
{
    protected override async Task<Response<LoadCountriesResult>> HandleCommandAsync(LoadCountriesCommand request,
        CancellationToken cancellationToken)
    {
        return await cacheDecorator.HandleAsync(request, ExecuteCommandAsync, cancellationToken);
    }

    private async Task<Response<LoadCountriesResult>> ExecuteCommandAsync(LoadCountriesCommand request,
        CancellationToken cancellationToken)
    {
        var existingCountries = await countryRepository.GetAllAsync(cancellationToken);

        if (existingCountries.Count != 0)
        {
            return HandleExistingCountries(existingCountries);
        }

        var countries = await LoadCountriesFromApiAsync(cancellationToken);
        
        var result = new LoadCountriesResult
        {
            CountriesLoaded = countries.Count,
            Countries = countries.Select(c => c.MapToDto()).ToList()
        };
        
        return new Response<LoadCountriesResult>(true, result,"Countries loaded successfully");
    }
    
    private Response<LoadCountriesResult> HandleExistingCountries(IList<Domain.Entities.Country> existingCountries)
    {
        var result = new LoadCountriesResult
        {
            CountriesLoaded = existingCountries.Count,
            Countries = existingCountries.Select(c => c.MapToDto()).ToList()
        };
        
        return new Response<LoadCountriesResult>(true, result,"Countries already loaded");
    }
    
    private async Task<List<Domain.Entities.Country>> LoadCountriesFromApiAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("Loading countries from API");
        var countriesDto = await apiClient.GetAvailableCountriesAsync();

        var countries = countriesDto.Select(c => c.Map()).ToList();

        await countryRepository.AddRangeAsync(countries, cancellationToken);
      
        return countries;
    }
}