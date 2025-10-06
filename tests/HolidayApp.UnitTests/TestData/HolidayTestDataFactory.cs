using HolidayApp.Application.Common.Models.Dtos;
using HolidayApp.Domain.Entities;

namespace HolidayApp.UnitTests.TestData;

public static class HolidayTestDataFactory
{
    public static Holiday CreateNewYearHoliday(int countryId, int year = 2024) => new()
    {
        CountryId = countryId,
        Date = new DateTime(year, 1, 1),
        LocalName = "New Year's Day",
        GlobalName = "New Year's Day",
        Year = year,
        IsWeekend = false
    };

    public static Holiday CreateIndependenceDayHoliday(int countryId, int year = 2024) => new()
    {
        CountryId = countryId,
        Date = new DateTime(year, 7, 4),
        LocalName = "Independence Day",
        GlobalName = "Independence Day",
        Year = year,
        IsWeekend = false
    };

    public static Holiday CreateChristmasHoliday(int countryId, int year = 2024) => new()
    {
        CountryId = countryId,
        Date = new DateTime(year, 12, 25),
        LocalName = "Christmas Day",
        GlobalName = "Christmas Day",
        Year = year,
        IsWeekend = false
    };

    public static Holiday CreateBoxingDayHoliday(int countryId, int year = 2024) => new()
    {
        CountryId = countryId,
        Date = new DateTime(year, 12, 26),
        LocalName = "Boxing Day",
        GlobalName = "Boxing Day",
        Year = year,
        IsWeekend = false
    };

    public static Holiday CreateWeekendHoliday(int countryId, DateTime date, string name) => new()
    {
        CountryId = countryId,
        Date = date,
        LocalName = name,
        GlobalName = name,
        Year = date.Year,
        IsWeekend = true
    };

    public static Holiday CreateHoliday(int id, int countryId, DateTime date, string localName, string globalName, bool isWeekend = false) => new()
    {
        Id = id,
        CountryId = countryId,
        Date = date,
        LocalName = localName,
        GlobalName = globalName,
        Year = date.Year,
        IsWeekend = isWeekend
    };

    public static List<Holiday> CreatePastHolidays(int countryId)
    {
        var today = DateTime.Today;
        return new List<Holiday>
        {
            CreateHoliday(1, countryId, today.AddDays(-10), "Holiday 1", "Holiday 1"),
            CreateHoliday(2, countryId, today.AddDays(-5), "Holiday 2", "Holiday 2"),
            CreateHoliday(3, countryId, today.AddDays(-2), "Holiday 3", "Holiday 3")
        };
    }

    private static PublicHolidayDto CreatePublicHolidayDto(DateTime date, string localName, string name) => new()
    {
        Date = date,
        LocalName = localName,
        Name = name
    };

    public static List<PublicHolidayDto> CreatePublicHolidayDtos() => new()
    {
        CreatePublicHolidayDto(new DateTime(2024, 1, 1), "New Year", "New Year's Day"),
        CreatePublicHolidayDto(new DateTime(2024, 7, 4), "Independence Day", "Independence Day")
    };
}
