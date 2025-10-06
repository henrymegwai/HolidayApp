using HolidayApp.Application.Common.Abstractions;
using HolidayApp.Application.Common.Decorators;
using HolidayApp.Application.Common.Interfaces;
using HolidayApp.Application.Common.Mapper;
using HolidayApp.Application.Common.Models;
using Microsoft.Extensions.Logging;

namespace HolidayApp.Application.Features.Holiday.Commands;

public class LoadHolidaysCommandHandler(
    IHolidayRepository holidayRepository,
    ICountryRepository countryRepository,
    INagerDateApiClient apiClient,
    ICacheInvalidationDecorator<LoadHolidaysCommand, LoadHolidaysResult> cacheInvalidationDecorator,
    ILogger<LoadHolidaysCommandHandler> logger)
    : BaseDecoratedCommandHandler<LoadHolidaysCommand, LoadHolidaysResult>(logger)
{
    private const int NoHolidaysLoadedCount = 0;
    
    protected override async Task<Response<LoadHolidaysResult>> HandleCommandAsync(LoadHolidaysCommand request,
        CancellationToken cancellationToken)
    {
        return await cacheInvalidationDecorator.HandleAsync(request, ExecuteCommandAsync, cancellationToken);
    }

    private async Task<Response<LoadHolidaysResult>> ExecuteCommandAsync(LoadHolidaysCommand request,
        CancellationToken cancellationToken)
    {
        var holidaysExist = await holidayRepository.HolidaysExistForYearAsync(request.CountryCode,
            request.Year,
            cancellationToken);

        if (holidaysExist)
        {
            var holidayResult = new LoadHolidaysResult(NoHolidaysLoadedCount);

            return new Response<LoadHolidaysResult>(true,
                holidayResult,
                $"Holidays for {request.CountryCode} in {request.Year} already exist");
        }

        var country = await countryRepository.GetByCodeAsync(request.CountryCode, cancellationToken);
        if (country == null)
        {
            logger.LogWarning("Country {CountryCode} not found", request.CountryCode);
            return new Response<LoadHolidaysResult>(false, null!,
                $"Country {request.CountryCode} not found. Please load countries first.");
        }

        var holidaysDto = await apiClient.GetPublicHolidaysAsync(request.Year, request.CountryCode);

        var holidays = holidaysDto.Select(h => h.Map(country.Id, request.Year)).ToList();

        await holidayRepository.BulkInsertHolidaysAsync(holidays, cancellationToken);

        var result = new LoadHolidaysResult(holidays.Count);

        return new Response<LoadHolidaysResult>(true, result, $"Successfully loaded {holidays.Count} holidays");
    }
}