using FluentAssertions;
using HolidayApp.Application.Common.Interfaces;
using HolidayApp.Application.Common.Models.Dtos;
using HolidayApp.Application.Features.Holiday.Queries.GetWeekdayHolidayCount;
using HolidayApp.UnitTests.TestData;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace HolidayApp.UnitTests.Application.Queries;

public class GetWeekdayHolidayCountQueryHandlerTests
{
    private readonly IHolidayRepository _holidayRepository;
    private readonly GetWeekdayHolidayCountQueryHandler _handler;

    public GetWeekdayHolidayCountQueryHandlerTests()
    {
        _holidayRepository = Substitute.For<IHolidayRepository>();
        var logger = Substitute.For<ILogger<GetWeekdayHolidayCountQueryHandler>>();

        _handler = new GetWeekdayHolidayCountQueryHandler(_holidayRepository, logger);
    }

    [Theory]
    [MemberData(nameof(QueryTestDataFactory.WeekdayHolidayCountScenarios), MemberType = typeof(QueryTestDataFactory))]
    public async Task HandleQueryAsyncShouldReturnCorrectCountsForDifferentScenarios(
        int year,
        string[] countryCodes,
        List<CountryHolidayCountDto> expectedCounts,
        int expectedResultCount)
    {
        // Arrange
        var query = new GetWeekdayHolidayCountQuery(year, countryCodes);
        _holidayRepository.GetWeekdayHolidayCountsAsync(Arg.Any<int>(), Arg.Any<string[]>(), Arg.Any<CancellationToken>())
            .Returns(expectedCounts);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Status.Should().BeTrue();
        result.Data.Should().HaveCount(expectedResultCount);
        result.Data.Should().BeEquivalentTo(expectedCounts);
    }

    [Fact]
    public async Task HandleQueryAsyncShouldReturnWeekdayHolidayCountsForMultipleCountries()
    {
        // Arrange
        var query = new GetWeekdayHolidayCountQuery(2024, new[] { "US", "GB", "CA" });
        var counts = QueryTestDataFactory.WeekdayHolidayCounts.CreateMultipleCountryCounts();

        _holidayRepository.GetWeekdayHolidayCountsAsync(Arg.Any<int>(), Arg.Any<string[]>(), Arg.Any<CancellationToken>())
            .Returns(counts);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Data.Should().HaveCount(3);
        result.Data.Should().Contain(c => c.CountryCode == "US" && c.HolidayCount == 11);
        result.Data.Should().Contain(c => c.CountryCode == "GB" && c.HolidayCount == 8);
        result.Data.Should().Contain(c => c.CountryCode == "CA" && c.HolidayCount == 9);
    }

    [Fact]
    public async Task HandleQueryAsyncShouldReturnEmptyListWhenNoCountriesMatch()
    {
        // Arrange
        var query = new GetWeekdayHolidayCountQuery(2024, new[] { "XX", "YY" });
        var emptyList = new List<CountryHolidayCountDto>();

        _holidayRepository.GetWeekdayHolidayCountsAsync(Arg.Any<int>(), Arg.Any<string[]>(), Arg.Any<CancellationToken>())
            .Returns(emptyList);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Data.Should().BeEmpty();
    }

    [Fact]
    public async Task HandleQueryAsyncShouldReturnCountForSingleCountry()
    {
        // Arrange
        var query = new GetWeekdayHolidayCountQuery(2024, new[] { "FR" });
        var counts = new List<CountryHolidayCountDto>
        {
            QueryTestDataFactory.WeekdayHolidayCounts.Create("FR", 11)
        };

        _holidayRepository.GetWeekdayHolidayCountsAsync(Arg.Any<int>(), Arg.Any<string[]>(), Arg.Any<CancellationToken>())
            .Returns(counts);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Data.Should().HaveCount(1);
        result.Data.First().CountryCode.Should().Be("FR");
        result.Data.First().HolidayCount.Should().Be(11);
    }

    [Theory]
    [InlineData(2024, new[] { "US", "GB" })]
    [InlineData(2023, new[] { "FR" })]
    [InlineData(2025, new[] { "DE", "CA", "US" })]
    public async Task HandleQueryAsyncShouldPassCorrectParametersToRepository(int year, string[] countryCodes)
    {
        // Arrange
        var query = new GetWeekdayHolidayCountQuery(year, countryCodes);
        var counts = new List<CountryHolidayCountDto>();

        _holidayRepository.GetWeekdayHolidayCountsAsync(year, countryCodes, Arg.Any<CancellationToken>())
            .Returns(counts);

        // Act
        await _handler.Handle(query, CancellationToken.None);

        // Assert
        await _holidayRepository.Received(1)
            .GetWeekdayHolidayCountsAsync(year,
                Arg.Is<string[]>(arr => arr.SequenceEqual(countryCodes)),
                Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task HandleQueryAsyncShouldReturnResultsInDescendingOrderByCount()
    {
        // Arrange
        var query = new GetWeekdayHolidayCountQuery(2024, new[] { "FR", "GB", "US", "CA" });
        var counts = QueryTestDataFactory.WeekdayHolidayCounts.CreateDescendingOrder();

        _holidayRepository.GetWeekdayHolidayCountsAsync(Arg.Any<int>(), Arg.Any<string[]>(), Arg.Any<CancellationToken>())
            .Returns(counts);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Data.Should().HaveCount(4);
        result.Data.Should().BeInDescendingOrder(c => c.HolidayCount);
        result.Data.First().CountryCode.Should().Be("FR");
        result.Data.First().HolidayCount.Should().Be(11);
    }

    [Fact]
    public async Task HandleQueryAsyncShouldReturnSuccessStatusAndMessage()
    {
        // Arrange
        var query = new GetWeekdayHolidayCountQuery(2024, new[] { "US", "GB" });
        var counts = new List<CountryHolidayCountDto>
        {
            QueryTestDataFactory.WeekdayHolidayCounts.Create("US", 11),
            QueryTestDataFactory.WeekdayHolidayCounts.Create("GB", 8)
        };

        _holidayRepository.GetWeekdayHolidayCountsAsync(Arg.Any<int>(), Arg.Any<string[]>(), Arg.Any<CancellationToken>())
            .Returns(counts);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Status.Should().BeTrue();
        result.Message.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task HandleQueryAsyncShouldHandleEmptyCountryCodesArray()
    {
        // Arrange
        var query = new GetWeekdayHolidayCountQuery(2024, Array.Empty<string>());
        var emptyList = new List<CountryHolidayCountDto>();

        _holidayRepository.GetWeekdayHolidayCountsAsync(Arg.Any<int>(), Arg.Any<string[]>(), Arg.Any<CancellationToken>())
            .Returns(emptyList);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Data.Should().BeEmpty();
    }

    [Theory]
    [InlineData(2020)]
    [InlineData(2024)]
    [InlineData(2030)]
    public async Task HandleQueryAsyncShouldWorkForDifferentYears(int year)
    {
        // Arrange
        var query = new GetWeekdayHolidayCountQuery(year, new[] { "US" });
        var counts = new List<CountryHolidayCountDto>
        {
            QueryTestDataFactory.WeekdayHolidayCounts.Create("US", 10)
        };

        _holidayRepository.GetWeekdayHolidayCountsAsync(Arg.Any<int>(), Arg.Any<string[]>(), Arg.Any<CancellationToken>())
            .Returns(counts);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Data.Should().HaveCount(1);

        await _holidayRepository.Received(1)
            .GetWeekdayHolidayCountsAsync(year, Arg.Any<string[]>(), Arg.Any<CancellationToken>());
    }
}
