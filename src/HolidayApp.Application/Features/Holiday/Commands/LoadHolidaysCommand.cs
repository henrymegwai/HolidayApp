using HolidayApp.Application.Common.Interfaces;
using HolidayApp.Application.Common.Models;

namespace HolidayApp.Application.Features.Holiday.Commands;

public record LoadHolidaysCommand(int Year, string CountryCode) : ICommand<Response<LoadHolidaysResult>>;

public record LoadHolidaysResult(int HolidaysLoaded);