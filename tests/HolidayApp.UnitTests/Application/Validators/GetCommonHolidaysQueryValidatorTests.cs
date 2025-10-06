using FluentAssertions;
using HolidayApp.Application.Features.Holiday.Queries.GetCommonHolidays;

namespace HolidayApp.UnitTests.Application.Validators;

public class GetCommonHolidaysQueryValidatorTests
{
    private readonly GetCommonHolidaysQueryValidator _validator = new();

    [Fact]
    public async Task ValidateShouldPassWhenAllFieldsAreValid()
    {
        // Arrange
        var query = new GetCommonHolidaysQuery(2024, "US", "CA");

        // Act
        var result = await _validator.ValidateAsync(query);

        // Assert
        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }

    [Theory]
    [InlineData(1899)]
    [InlineData(1900)]
    public async Task ValidateShouldFailWhenYearIsNotGreaterThan1900(int year)
    {
        // Arrange
        var query = new GetCommonHolidaysQuery(year, "US", "CA");

        // Act
        var result = await _validator.ValidateAsync(query);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Year");
    }

    [Theory]
    [InlineData("", "CA")]
    [InlineData("US", "")]
    [InlineData(null, "CA")]
    [InlineData("US", null)]
    public async Task ValidateShouldFailWhenCountryCodesAreEmpty(string? code1, string? code2)
    {
        // Arrange
        var query = new GetCommonHolidaysQuery(2024, code1!, code2!);

        // Act
        var result = await _validator.ValidateAsync(query);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.ErrorMessage.Contains("required"));
    }

    [Theory]
    [InlineData("U", "CA")]
    [InlineData("US", "C")]
    [InlineData("USA", "CA")]
    [InlineData("US", "CAN")]
    public async Task ValidateShouldFailWhenCountryCodesAreNotExactlyTwoCharacters(string code1, string code2)
    {
        // Arrange
        var query = new GetCommonHolidaysQuery(2024, code1, code2);

        // Act
        var result = await _validator.ValidateAsync(query);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.ErrorMessage.Contains("exactly 2 characters"));
    }

    [Theory]
    [InlineData("us", "CA")]
    [InlineData("US", "ca")]
    [InlineData("12", "CA")]
    [InlineData("US", "C1")]
    public async Task ValidateShouldFailWhenCountryCodesAreNotUppercaseLetters(string code1, string code2)
    {
        // Arrange
        var query = new GetCommonHolidaysQuery(2024, code1, code2);

        // Act
        var result = await _validator.ValidateAsync(query);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.ErrorMessage.Contains("2 uppercase letters"));
    }

    [Fact]
    public async Task ValidateShouldFailWhenCountryCodesAreTheSame()
    {
        // Arrange
        var query = new GetCommonHolidaysQuery(2024, "US", "US");

        // Act
        var result = await _validator.ValidateAsync(query);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.ErrorMessage.Contains("must be different"));
    }

    [Theory]
    [InlineData("US", "CA")]
    [InlineData("GB", "FR")]
    [InlineData("DE", "IT")]
    public async Task ValidateShouldPassWhenCountryCodesAreDifferentAndValid(string code1, string code2)
    {
        // Arrange
        var query = new GetCommonHolidaysQuery(2024, code1, code2);

        // Act
        var result = await _validator.ValidateAsync(query);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task ValidateShouldNotCheckDifferenceWhenCountryCodesAreInvalid()
    {
        // Arrange
        var query = new GetCommonHolidaysQuery(2024, "us", "us");

        // Act
        var result = await _validator.ValidateAsync(query);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.ErrorMessage.Contains("2 uppercase letters"));
    }
}
