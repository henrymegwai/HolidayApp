using FluentAssertions;
using HolidayApp.Application.Features.Holiday.Commands;
using HolidayApp.UnitTests.TestData;

namespace HolidayApp.UnitTests.Application.Validators;

public class LoadHolidaysCommandValidatorTests
{
    private readonly LoadHolidaysCommandValidator _validator = new();

    [Theory]
    [InlineData(2024, "US")]
    [InlineData(2025, "GB")]
    [InlineData(1901, "CA")]
    public async Task ValidateShouldPassWhenYearAndCountryCodeAreValid(int year, string countryCode)
    {
        // Arrange
        var command = new LoadHolidaysCommand(year, countryCode);

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }

    [Theory]
    [InlineData(1899)]
    [InlineData(1900)]
    [InlineData(0)]
    [InlineData(-2024)]
    public async Task ValidateShouldFailWhenYearIsLessThanOrEqualTo1900(int year)
    {
        // Arrange
        var command = new LoadHolidaysCommand(year, "US");

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Year" && e.ErrorMessage.Contains("greater than 1900"));
    }

    [Fact]
    public async Task ValidateShouldFailWhenYearIsMoreThan10YearsInFuture()
    {
        // Arrange
        var futureYear = DateTime.Now.Year + 11;
        var command = new LoadHolidaysCommand(futureYear, "US");

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Year" && e.ErrorMessage.Contains("10 years in the future"));
    }

    [Theory]
    [MemberData(nameof(ValidationTestData.ValidYearData), MemberType = typeof(ValidationTestData))]
    public async Task ValidateShouldPassForValidYears(int year)
    {
        // Arrange
        var command = new LoadHolidaysCommand(year, "US");

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Theory]
    [MemberData(nameof(ValidationTestData.InvalidCountryCodeData), MemberType = typeof(ValidationTestData))]
    public async Task ValidateShouldFailForInvalidCountryCodes(string countryCode, string expectedErrorMessage)
    {
        // Arrange
        var command = new LoadHolidaysCommand(2024, countryCode);

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "CountryCode" && e.ErrorMessage.Contains(expectedErrorMessage));
    }

    [Theory]
    [MemberData(nameof(ValidationTestData.ValidCountryCodeData), MemberType = typeof(ValidationTestData))]
    public async Task ValidateShouldPassForValidCountryCodes(string countryCode)
    {
        // Arrange
        var command = new LoadHolidaysCommand(2024, countryCode);

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task ValidateShouldFailWhenCountryCodeIsNull()
    {
        // Arrange
        var command = new LoadHolidaysCommand(2024, null!);

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "CountryCode");
    }

    [Theory]
    [InlineData(1901, "US", true)]
    [InlineData(1900, "US", false)]
    [InlineData(2024, "", false)]
    [InlineData(2024, "USA", false)]
    [InlineData(2024, "us", false)]
    public async Task ValidateShouldHandleMultipleValidationScenarios(int year, string countryCode, bool expectedValid)
    {
        // Arrange
        var command = new LoadHolidaysCommand(year, countryCode);

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        result.IsValid.Should().Be(expectedValid);
    }
}
