using HolidayApp.Application.Common.Interfaces;
using HolidayApp.Application.Common.Models;
using HolidayApp.Application.Common.Models.Dtos;

namespace HolidayApp.Application.Features.Holiday.Queries.GetCommonHolidays;

public record GetCommonHolidaysQuery(int Year, string CountryCode1, string CountryCode2)
    : IQuery<Response<List<CommonHolidayDto>>>;