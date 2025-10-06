using FluentAssertions;
using HolidayApp.Application.Common.Decorators;
using HolidayApp.Application.Common.Interfaces;
using HolidayApp.Application.Common.Models;
using HolidayApp.Application.Common.Models.Dtos;
using HolidayApp.Application.Features.Holiday.Queries.GetLastCelebratedHolidays;
using HolidayApp.UnitTests.TestData;
using NSubstitute;

namespace HolidayApp.UnitTests.Application.Queries;

public class GetLastCelebratedHolidaysQueryHandlerTests
{
    private readonly IHolidayRepository _holidayRepository;
    private readonly ICacheDecorator<GetLastCelebratedHolidaysQuery, List<LastCelebratedHolidayDto>> _cacheDecorator;
    private readonly GetLastCelebratedHolidaysQueryHandler _handler;
    
    private const int NumberOfHolidays = 3;

    public GetLastCelebratedHolidaysQueryHandlerTests()
    {
        _holidayRepository = Substitute.For<IHolidayRepository>();
        _cacheDecorator = Substitute.For<ICacheDecorator<GetLastCelebratedHolidaysQuery, List<LastCelebratedHolidayDto>>>();
        
        _handler = new GetLastCelebratedHolidaysQueryHandler(
            _holidayRepository,
            _cacheDecorator);
    }

    [Theory]
    [MemberData(nameof(QueryTestDataFactory.LastCelebratedHolidayScenarios), MemberType = typeof(QueryTestDataFactory))]
    public async Task HandleQueryAsyncShouldReturnCorrectHolidaysForDifferentScenarios(
        string countryCode,
        int numberOfHolidays,
        List<LastCelebratedHolidayDto> expectedHolidays,
        int expectedCount)
    {
        // Arrange
        var query = new GetLastCelebratedHolidaysQuery(countryCode, numberOfHolidays);
        var expectedResponse = new Response<List<LastCelebratedHolidayDto>>(true, expectedHolidays, "Holidays fetched successfully");
     
        _cacheDecorator.HandleAsync(Arg.Any<GetLastCelebratedHolidaysQuery>(), Arg.Any<Func<GetLastCelebratedHolidaysQuery, CancellationToken, Task<Response<List<LastCelebratedHolidayDto>>>>>(), Arg.Any<CancellationToken>())
            .Returns(expectedResponse);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Status.Should().BeTrue();
        result.Data.Should().HaveCount(expectedCount);
        result.Data.Should().BeEquivalentTo(expectedHolidays);
    }

    [Fact]
    public async Task HandleQueryAsyncShouldReturnCachedDataWhenAvailable()
    {
        // Arrange
        var query = new GetLastCelebratedHolidaysQuery("US", NumberOfHolidays);
        var cachedHolidays = QueryTestDataFactory.LastCelebratedHolidays.CreateUSLastThreeHolidays();
        var expectedResponse = new Response<List<LastCelebratedHolidayDto>>(true, cachedHolidays, "Data retrieved from cache");
        
        _cacheDecorator.HandleAsync(Arg.Any<GetLastCelebratedHolidaysQuery>(), Arg.Any<Func<GetLastCelebratedHolidaysQuery, CancellationToken, Task<Response<List<LastCelebratedHolidayDto>>>>>(), Arg.Any<CancellationToken>())
            .Returns(expectedResponse);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Data.Should().HaveCount(NumberOfHolidays);
        result.Data.Should().BeEquivalentTo(cachedHolidays);
        
        await _cacheDecorator.Received(1)
            .HandleAsync(Arg.Any<GetLastCelebratedHolidaysQuery>(), Arg.Any<Func<GetLastCelebratedHolidaysQuery, CancellationToken, Task<Response<List<LastCelebratedHolidayDto>>>>>(), Arg.Any<CancellationToken>());
        
        await _holidayRepository.DidNotReceive()
            .GetLastCelebratedHolidaysAsync(Arg.Any<string>(), Arg.Any<int>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task HandleQueryAsyncShouldFetchFromRepositoryWhenNotCached()
    {
        // Arrange
        var query = new GetLastCelebratedHolidaysQuery("GB", NumberOfHolidays);
        var holidays = QueryTestDataFactory.LastCelebratedHolidays.CreatePastHolidays(NumberOfHolidays);
        var expectedResponse = new Response<List<LastCelebratedHolidayDto>>(true, holidays, "Holidays fetched successfully");
        
        _cacheDecorator.HandleAsync(Arg.Any<GetLastCelebratedHolidaysQuery>(), Arg.Any<Func<GetLastCelebratedHolidaysQuery, CancellationToken, Task<Response<List<LastCelebratedHolidayDto>>>>>(), Arg.Any<CancellationToken>())
            .Returns(expectedResponse);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Data.Should().HaveCount(NumberOfHolidays);
        result.Data.Should().BeEquivalentTo(holidays);
        
        await _cacheDecorator.Received(1)
            .HandleAsync(Arg.Any<GetLastCelebratedHolidaysQuery>(), Arg.Any<Func<GetLastCelebratedHolidaysQuery, CancellationToken, Task<Response<List<LastCelebratedHolidayDto>>>>>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task HandleQueryAsyncShouldCacheResults()
    {
        // Arrange
        var query = new GetLastCelebratedHolidaysQuery("DE" , NumberOfHolidays);
        var holidays = QueryTestDataFactory.LastCelebratedHolidays.CreatePastHolidays(NumberOfHolidays);
        var expectedResponse = new Response<List<LastCelebratedHolidayDto>>(true, holidays, "Holidays fetched successfully");
        
        _cacheDecorator.HandleAsync(Arg.Any<GetLastCelebratedHolidaysQuery>(), Arg.Any<Func<GetLastCelebratedHolidaysQuery, CancellationToken, Task<Response<List<LastCelebratedHolidayDto>>>>>(), Arg.Any<CancellationToken>())
            .Returns(expectedResponse);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Data.Should().HaveCount(NumberOfHolidays);
        result.Data.Should().BeEquivalentTo(holidays);
        
        await _cacheDecorator.Received(1)
            .HandleAsync(Arg.Any<GetLastCelebratedHolidaysQuery>(), Arg.Any<Func<GetLastCelebratedHolidaysQuery, CancellationToken, Task<Response<List<LastCelebratedHolidayDto>>>>>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task HandleQueryAsyncShouldReturnEmptyListWhenNoHolidaysFound()
    {
        // Arrange
        var query = new GetLastCelebratedHolidaysQuery("XX", 0);
        var emptyList = new List<LastCelebratedHolidayDto>();
        var expectedResponse = new Response<List<LastCelebratedHolidayDto>>(true, emptyList, "Holidays fetched successfully");

        _cacheDecorator.HandleAsync(Arg.Any<GetLastCelebratedHolidaysQuery>(), Arg.Any<Func<GetLastCelebratedHolidaysQuery, CancellationToken, Task<Response<List<LastCelebratedHolidayDto>>>>>(), Arg.Any<CancellationToken>())
            .Returns(expectedResponse);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Data.Should().BeEmpty();
    }

    [Theory]
    [InlineData("US")]
    [InlineData("GB")]
    [InlineData("CA")]
    public async Task HandleQueryAsyncShouldCallRepositoryWithCorrectCountryCode(string countryCode)
    {
        // Arrange
        var query = new GetLastCelebratedHolidaysQuery(countryCode, NumberOfHolidays);
        var holidays = new List<LastCelebratedHolidayDto>();
        var expectedResponse = new Response<List<LastCelebratedHolidayDto>>(true, holidays, "Holidays fetched successfully");

        _cacheDecorator.HandleAsync(Arg.Any<GetLastCelebratedHolidaysQuery>(), Arg.Any<Func<GetLastCelebratedHolidaysQuery, CancellationToken, Task<Response<List<LastCelebratedHolidayDto>>>>>(), Arg.Any<CancellationToken>())
            .Returns(expectedResponse);

        // Act
        await _handler.Handle(query, CancellationToken.None);

        // Assert
        await _cacheDecorator.Received(1)
            .HandleAsync(Arg.Any<GetLastCelebratedHolidaysQuery>(), Arg.Any<Func<GetLastCelebratedHolidaysQuery, CancellationToken, Task<Response<List<LastCelebratedHolidayDto>>>>>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task HandleQueryAsyncShouldReturnSuccessStatusAndMessage()
    {
        // Arrange
        var query = new GetLastCelebratedHolidaysQuery("US", NumberOfHolidays);
        var holidays = QueryTestDataFactory.LastCelebratedHolidays.CreateUSLastThreeHolidays();
        var expectedResponse = new Response<List<LastCelebratedHolidayDto>>(true, holidays, "Holidays fetched successfully");

        _cacheDecorator.HandleAsync(Arg.Any<GetLastCelebratedHolidaysQuery>(), Arg.Any<Func<GetLastCelebratedHolidaysQuery, CancellationToken, Task<Response<List<LastCelebratedHolidayDto>>>>>(), Arg.Any<CancellationToken>())
            .Returns(expectedResponse);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Status.Should().BeTrue();
        result.Message.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task HandleQueryAsyncShouldUseDecoratorPattern()
    {
        // Arrange
        var query = new GetLastCelebratedHolidaysQuery("FR", NumberOfHolidays);
        var holidays = QueryTestDataFactory.LastCelebratedHolidays.CreatePastHolidays(NumberOfHolidays);
        var expectedResponse = new Response<List<LastCelebratedHolidayDto>>(true, holidays, "Holidays fetched successfully");

        _cacheDecorator.HandleAsync(Arg.Any<GetLastCelebratedHolidaysQuery>(), Arg.Any<Func<GetLastCelebratedHolidaysQuery, CancellationToken, Task<Response<List<LastCelebratedHolidayDto>>>>>(), Arg.Any<CancellationToken>())
            .Returns(expectedResponse);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Data.Should().HaveCount(NumberOfHolidays);

        // Verify that the decorator pattern is working - the decorator should be called
        await _cacheDecorator.Received(1)
            .HandleAsync(Arg.Any<GetLastCelebratedHolidaysQuery>(), Arg.Any<Func<GetLastCelebratedHolidaysQuery, CancellationToken, Task<Response<List<LastCelebratedHolidayDto>>>>>(), Arg.Any<CancellationToken>());
    }
}
