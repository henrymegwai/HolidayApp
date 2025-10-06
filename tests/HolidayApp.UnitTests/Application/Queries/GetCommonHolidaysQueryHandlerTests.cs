using FluentAssertions;
using HolidayApp.Application.Common.Interfaces;
using HolidayApp.Application.Common.Models.Dtos;
using HolidayApp.Application.Features.Holiday.Queries.GetCommonHolidays;
using HolidayApp.UnitTests.TestData;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace HolidayApp.UnitTests.Application.Queries;

public class GetCommonHolidaysQueryHandlerTests
{
    private readonly IHolidayRepository _holidayRepository;
    private readonly GetCommonHolidaysQueryHandler _handler;

    public GetCommonHolidaysQueryHandlerTests()
    {
        _holidayRepository = Substitute.For<IHolidayRepository>();
        var logger = Substitute.For<ILogger<GetCommonHolidaysQueryHandler>>();

        _handler = new GetCommonHolidaysQueryHandler(_holidayRepository, logger);
    }

    [Theory]
    [MemberData(nameof(QueryTestDataFactory.CommonHolidayScenarios), MemberType = typeof(QueryTestDataFactory))]
    public async Task HandleQueryAsyncShouldReturnCorrectCommonHolidaysForDifferentScenarios(
        int year,
        string countryCode1,
        string countryCode2,
        List<CommonHolidayDto> expectedHolidays,
        int expectedCount)
    {
        // Arrange
        var query = new GetCommonHolidaysQuery(year, countryCode1, countryCode2);
        _holidayRepository.GetCommonHolidaysAsync(year, countryCode1, countryCode2, Arg.Any<CancellationToken>())
            .Returns(expectedHolidays);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Status.Should().BeTrue();
        result.Data.Should().HaveCount(expectedCount);
        result.Data.Should().BeEquivalentTo(expectedHolidays);
    }

    [Fact]
    public async Task HandleQueryAsyncShouldReturnCommonHolidaysBetweenTwoCountries()
    {
        // Arrange
        var query = new GetCommonHolidaysQuery(2024, "US", "CA");
        var commonHolidays = QueryTestDataFactory.CommonHolidays.CreateUSCanadaCommonHolidays();

        _holidayRepository.GetCommonHolidaysAsync(2024, "US", "CA", Arg.Any<CancellationToken>())
            .Returns(commonHolidays);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Data.Should().HaveCount(2);
        result.Data.Should().Contain(h => h.Date == new DateTime(2024, 1, 1));
        result.Data.Should().Contain(h => h.Date == new DateTime(2024, 12, 25));
    }

    [Fact]
    public async Task HandleQueryAsyncShouldReturnEmptyListWhenNoCommonHolidays()
    {
        // Arrange
        var query = new GetCommonHolidaysQuery(2024, "US", "JP");
        var emptyList = new List<CommonHolidayDto>();

        _holidayRepository.GetCommonHolidaysAsync(2024, "US", "JP", Arg.Any<CancellationToken>())
            .Returns(emptyList);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Data.Should().BeEmpty();
    }

    [Theory]
    [InlineData(2023, "GB", "FR")]
    [InlineData(2024, "US", "CA")]
    [InlineData(2025, "DE", "FR")]
    public async Task HandleQueryAsyncShouldPassCorrectParametersToRepository(int year, string code1, string code2)
    {
        // Arrange
        var query = new GetCommonHolidaysQuery(year, code1, code2);
        var holidays = new List<CommonHolidayDto>();

        _holidayRepository.GetCommonHolidaysAsync(year, code1, code2, Arg.Any<CancellationToken>())
            .Returns(holidays);

        // Act
        await _handler.Handle(query, CancellationToken.None);

        // Assert
        await _holidayRepository.Received(1)
            .GetCommonHolidaysAsync(year, code1, code2, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task HandleQueryAsyncShouldReturnHolidaysWithDifferentLocalNames()
    {
        // Arrange
        var query = new GetCommonHolidaysQuery(2024, "US", "MX");
        var commonHolidays = QueryTestDataFactory.CommonHolidays.CreateUSMexicoCommonHolidays();

        _holidayRepository.GetCommonHolidaysAsync(2024, "US", "MX", Arg.Any<CancellationToken>())
            .Returns(commonHolidays);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Data.Should().HaveCount(1);
        result.Data.First().LocalName1.Should().Be("New Year's Day");
        result.Data.First().LocalName2.Should().Be("Año Nuevo");
    }

    [Fact]
    public async Task HandleQueryAsyncShouldReturnSuccessStatusAndMessage()
    {
        // Arrange
        var query = new GetCommonHolidaysQuery(2024, "US", "CA");
        var holidays = QueryTestDataFactory.CommonHolidays.CreateUSCanadaCommonHolidays();

        _holidayRepository.GetCommonHolidaysAsync(2024, "US", "CA", Arg.Any<CancellationToken>())
            .Returns(holidays);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Status.Should().BeTrue();
        result.Message.Should().NotBeNullOrEmpty();
    }
}
