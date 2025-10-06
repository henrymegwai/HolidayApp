using HolidayApp.Application.Common.Abstractions;
using HolidayApp.Application.Common.Interfaces;
using HolidayApp.Application.Common.Models;
using HolidayApp.Application.Common.Models.Dtos;
using Microsoft.Extensions.Logging;

namespace HolidayApp.Application.Features.Holiday.Queries.GetWeekdayHolidayCount;

public class GetWeekdayHolidayCountQueryHandler(
    IHolidayRepository holidayRepository,
    ILogger<GetWeekdayHolidayCountQueryHandler> logger)
    : BaseQueryHandler<GetWeekdayHolidayCountQuery, Response<List<CountryHolidayCountDto>>>(logger)
{
    protected override async Task<Response<List<CountryHolidayCountDto>>> HandleQueryAsync(
        GetWeekdayHolidayCountQuery request,
        CancellationToken cancellationToken)
    {
        var results = await holidayRepository.GetWeekdayHolidayCountsAsync(
            request.Year,
            request.CountryCodes,
            cancellationToken);

        return new Response<List<CountryHolidayCountDto>>(true, results, "Weekday holidays fetched successfully");
    }
}
