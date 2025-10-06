using HolidayApp.Application.Common.Interfaces;
using HolidayApp.Application.Common.Models;
using HolidayApp.Application.Common.Models.Dtos;

namespace HolidayApp.Application.Features.Holiday.Queries.GetWeekdayHolidayCount;

public record GetWeekdayHolidayCountQuery(int Year, string[] CountryCodes) : IQuery<Response<List<CountryHolidayCountDto>>>;