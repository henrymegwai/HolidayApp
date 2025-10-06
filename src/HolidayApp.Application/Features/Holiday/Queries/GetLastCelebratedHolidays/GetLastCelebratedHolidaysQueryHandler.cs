using HolidayApp.Application.Common.Abstractions;
using HolidayApp.Application.Common.Decorators;
using HolidayApp.Application.Common.Interfaces;
using HolidayApp.Application.Common.Models;
using HolidayApp.Application.Common.Models.Dtos;

namespace HolidayApp.Application.Features.Holiday.Queries.GetLastCelebratedHolidays;

public class GetLastCelebratedHolidaysQueryHandler(
    IHolidayRepository holidayRepository,
    ICacheDecorator<GetLastCelebratedHolidaysQuery, List<LastCelebratedHolidayDto>> cacheDecorator)
    : BaseDecoratedQueryHandler<GetLastCelebratedHolidaysQuery, List<LastCelebratedHolidayDto>>
{
    protected override async Task<Response<List<LastCelebratedHolidayDto>>> HandleQueryAsync(
        GetLastCelebratedHolidaysQuery request,
        CancellationToken cancellationToken)
    {
        return await cacheDecorator.HandleAsync(request, ExecuteQueryAsync, cancellationToken);
    }

    private async Task<Response<List<LastCelebratedHolidayDto>>> ExecuteQueryAsync(
        GetLastCelebratedHolidaysQuery request,
        CancellationToken cancellationToken)
    {
        var holidays = await holidayRepository.GetLastCelebratedHolidaysAsync(
            request.CountryCode,
            request.NumberOfHolidays,
            cancellationToken);
        
        return new Response<List<LastCelebratedHolidayDto>>(true, holidays, "Holidays fetched successfully");
    }
}