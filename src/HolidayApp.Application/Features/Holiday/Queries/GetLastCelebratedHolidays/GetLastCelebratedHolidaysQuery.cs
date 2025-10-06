using HolidayApp.Application.Common.Interfaces;
using HolidayApp.Application.Common.Models;
using HolidayApp.Application.Common.Models.Dtos;

namespace HolidayApp.Application.Features.Holiday.Queries.GetLastCelebratedHolidays;

public record GetLastCelebratedHolidaysQuery(
    string CountryCode,
    int NumberOfHolidays) : IQuery<Response<List<LastCelebratedHolidayDto>>>;