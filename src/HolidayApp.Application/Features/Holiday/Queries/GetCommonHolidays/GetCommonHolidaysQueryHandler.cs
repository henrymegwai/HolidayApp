using HolidayApp.Application.Common.Abstractions;
using HolidayApp.Application.Common.Interfaces;
using HolidayApp.Application.Common.Models;
using HolidayApp.Application.Common.Models.Dtos;
using Microsoft.Extensions.Logging;

namespace HolidayApp.Application.Features.Holiday.Queries.GetCommonHolidays;

public class GetCommonHolidaysQueryHandler(
    IHolidayRepository holidayRepository,
    ILogger<GetCommonHolidaysQueryHandler> logger)
    : BaseQueryHandler<GetCommonHolidaysQuery, Response<List<CommonHolidayDto>>>(logger)
{
    protected override async Task<Response<List<CommonHolidayDto>>> HandleQueryAsync(
        GetCommonHolidaysQuery request,
        CancellationToken cancellationToken)
    {
        var commonHolidays = await holidayRepository.GetCommonHolidaysAsync(
            request.Year,
            request.CountryCode1,
            request.CountryCode2,
            cancellationToken);

        return new Response<List<CommonHolidayDto>>(true, commonHolidays, "Common holidays fetched successfully");
    }
}