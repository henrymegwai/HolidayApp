using HolidayApp.Application.Common.Models.Dtos;

namespace HolidayApp.UnitTests.TestData;

public static class QueryTestDataFactory
{
    public static class CommonHolidays
    {
        public static List<CommonHolidayDto> CreateUSCanadaCommonHolidays() =>
        [
            CreateNewYear(),
            CreateChristmas()
        ];

        public static List<CommonHolidayDto> CreateUSMexicoCommonHolidays() =>
            [Create(new DateTime(2024, 1, 1), "New Year's Day", "Año Nuevo")];
        
        
        private static CommonHolidayDto CreateNewYear(string localName1 = "New Year's Day", string localName2 = "New Year's Day") => new()
        {
            Date = new DateTime(2024, 1, 1),
            LocalName1 = localName1,
            LocalName2 = localName2
        };

        private static CommonHolidayDto CreateChristmas(string localName1 = "Christmas", string localName2 = "Christmas Day") => new()
        {
            Date = new DateTime(2024, 12, 25),
            LocalName1 = localName1,
            LocalName2 = localName2
        };

        private static CommonHolidayDto Create(DateTime date, string localName1, string localName2) => new()
        {
            Date = date,
            LocalName1 = localName1,
            LocalName2 = localName2
        };
    }

    public static class LastCelebratedHolidays
    {
        private static LastCelebratedHolidayDto Create(DateTime date, string name) => new()
        {
            Date = date,
            Name = name
        };

        public static List<LastCelebratedHolidayDto> CreateUSLastThreeHolidays()
        {
            var today = DateTime.Today;
            return
            [
                Create(today.AddDays(-2), "Recent Holiday 1"),
                Create(today.AddDays(-7), "Recent Holiday 2"),
                Create(today.AddDays(-14), "Recent Holiday 3")
            ];
        }

        public static List<LastCelebratedHolidayDto> CreatePastHolidays(int count)
        {
            var today = DateTime.Today;
            var holidays = new List<LastCelebratedHolidayDto>();

            for (var i = 0; i < count; i++)
            {
                holidays.Add(Create(today.AddDays(-(i + 1) * 7), $"Holiday {i + 1}"));
            }

            return holidays;
        }
    }

    public static class WeekdayHolidayCounts
    {
        public static CountryHolidayCountDto Create(string countryCode, int holidayCount) => new()
        {
            CountryCode = countryCode,
            HolidayCount = holidayCount
        };

        public static List<CountryHolidayCountDto> CreateMultipleCountryCounts() => new()
        {
            Create("GB", 8),
            Create("US", 11),
            Create("CA", 9)
        };

        public static List<CountryHolidayCountDto> CreateDescendingOrder() => new()
        {
            Create("FR", 11),
            Create("US", 10),
            Create("CA", 9),
            Create("GB", 8)
        };
    }

    // MemberData providers
    public static IEnumerable<object[]> CommonHolidayScenarios()
    {
        yield return
        [
            2024,
            "US",
            "CA",
            CommonHolidays.CreateUSCanadaCommonHolidays(),
            2
        ];

        yield return
        [
            2024,
            "US",
            "MX",
            CommonHolidays.CreateUSMexicoCommonHolidays(),
            1
        ];

        yield return
        [
            2024,
            "US",
            "JP",
            new List<CommonHolidayDto>(),
            0
        ];
    }

    public static IEnumerable<object[]> WeekdayHolidayCountScenarios()
    {
        yield return
        [
            2024,
            new[] { "US", "GB", "CA" },
            WeekdayHolidayCounts.CreateMultipleCountryCounts(),
            3
        ];

        yield return
        [
            2023,
            new[] { "FR" },
            new List<CountryHolidayCountDto> { WeekdayHolidayCounts.Create("FR", 11) },
            1
        ];

        yield return
        [
            2024,
            new[] { "XX", "YY" },
            new List<CountryHolidayCountDto>(),
            0
        ];
    }

    public static IEnumerable<object[]> LastCelebratedHolidayScenarios()
    {
        yield return
        [
            "US",
            3,
            LastCelebratedHolidays.CreateUSLastThreeHolidays(),
            3
        ];

        yield return
        [
            "GB",
            5,
            LastCelebratedHolidays.CreatePastHolidays(3),
            3
        ];

        yield return
        [
            "XX",
            3,
            new List<LastCelebratedHolidayDto>(),
            0
        ];
    }
}
