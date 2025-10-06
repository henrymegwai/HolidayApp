using HolidayApp.Domain.Entities;

namespace HolidayApp.UnitTests.TestData;

public static class CountryTestDataFactory
{
    public static Country CreateUnitedStates() => new()
    {
        Id = 1,
        CountryCode = "US",
        Name = "United States"
    };

    public static Country CreateUnitedKingdom() => new()
    {
        Id = 2,
        CountryCode = "GB",
        Name = "United Kingdom"
    };

    public static Country CreateCanada() => new()
    {
        Id = 3,
        CountryCode = "CA",
        Name = "Canada"
    };

    public static Country CreateGermany() => new()
    {
        Id = 4,
        CountryCode = "DE",
        Name = "Germany"
    };

    public static Country CreateFrance() => new()
    {
        Id = 5,
        CountryCode = "FR",
        Name = "France"
    };

    public static Country CreateCountry(int id, string countryCode, string name) => new()
    {
        Id = id,
        CountryCode = countryCode,
        Name = name
    };

    public static List<Country> CreateMultipleCountries() =>
    [
        CreateUnitedStates(),
        CreateUnitedKingdom(),
        CreateCanada(),
        CreateGermany(),
        CreateFrance()
    ];
}
