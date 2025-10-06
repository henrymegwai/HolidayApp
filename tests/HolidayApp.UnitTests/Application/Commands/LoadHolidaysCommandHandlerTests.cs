using FluentAssertions;
using HolidayApp.Application.Common.Decorators;
using HolidayApp.Application.Common.Interfaces;
using HolidayApp.Application.Common.Models;
using HolidayApp.Application.Features.Holiday.Commands;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace HolidayApp.UnitTests.Application.Commands;

public class LoadHolidaysCommandHandlerTests
{
    private readonly ICacheInvalidationDecorator<LoadHolidaysCommand, LoadHolidaysResult> _cacheInvalidationDecorator;
    private readonly LoadHolidaysCommandHandler _handler;

    public LoadHolidaysCommandHandlerTests()
    {
        var holidayRepository = Substitute.For<IHolidayRepository>();
        var countryRepository = Substitute.For<ICountryRepository>();
        var apiClient = Substitute.For<INagerDateApiClient>();
        _cacheInvalidationDecorator = Substitute.For<ICacheInvalidationDecorator<LoadHolidaysCommand, LoadHolidaysResult>>();
        var logger = Substitute.For<ILogger<LoadHolidaysCommandHandler>>();

        _handler = new LoadHolidaysCommandHandler(
            holidayRepository,
            countryRepository,
            apiClient,
            _cacheInvalidationDecorator,
            logger);
    }

    [Fact]
    public async Task HandleCommandAsyncShouldReturnSuccessWhenHolidaysAlreadyExist()
    {
        // Arrange
        var command = new LoadHolidaysCommand(2024, "US");
        var expectedResult = new Response<LoadHolidaysResult>(true, new LoadHolidaysResult(0), "Holidays for US in 2024 already exist");
        
        _cacheInvalidationDecorator.HandleAsync(Arg.Any<LoadHolidaysCommand>(), Arg.Any<Func<LoadHolidaysCommand, CancellationToken, Task<Response<LoadHolidaysResult>>>>(), Arg.Any<CancellationToken>())
            .Returns(expectedResult);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Status.Should().BeTrue();
        result.Data.HolidaysLoaded.Should().Be(0);
        result.Message.Should().Contain("already exist");

        await _cacheInvalidationDecorator.Received(1)
            .HandleAsync(Arg.Any<LoadHolidaysCommand>(), Arg.Any<Func<LoadHolidaysCommand, CancellationToken, Task<Response<LoadHolidaysResult>>>>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task HandleCommandAsyncShouldReturnFailureWhenCountryNotFound()
    {
        // Arrange
        var command = new LoadHolidaysCommand(2024, "XX");
        var expectedResult = new Response<LoadHolidaysResult>(false, null!, "Country XX not found. Please load countries first.");
        
        _cacheInvalidationDecorator.HandleAsync(Arg.Any<LoadHolidaysCommand>(), Arg.Any<Func<LoadHolidaysCommand, CancellationToken, Task<Response<LoadHolidaysResult>>>>(), Arg.Any<CancellationToken>())
            .Returns(expectedResult);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Status.Should().BeFalse();
        result.Message.Should().Contain("not found");
        result.Message.Should().Contain("load countries first");
    }

    [Fact]
    public async Task HandleCommandAsyncShouldLoadAndSaveHolidaysWhenValid()
    {
        // Arrange
        var command = new LoadHolidaysCommand(2024, "US");
        var expectedResult = new Response<LoadHolidaysResult>(true, new LoadHolidaysResult(2), "Successfully loaded 2 holidays");
        
        _cacheInvalidationDecorator.HandleAsync(Arg.Any<LoadHolidaysCommand>(), Arg.Any<Func<LoadHolidaysCommand, CancellationToken, Task<Response<LoadHolidaysResult>>>>(), Arg.Any<CancellationToken>())
            .Returns(expectedResult);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Status.Should().BeTrue();
        result.Data.HolidaysLoaded.Should().Be(2);
        result.Message.Should().Contain("Successfully loaded 2 holidays");
        
        await _cacheInvalidationDecorator.Received(1)
            .HandleAsync(Arg.Any<LoadHolidaysCommand>(), Arg.Any<Func<LoadHolidaysCommand, CancellationToken, Task<Response<LoadHolidaysResult>>>>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task HandleCommandAsyncShouldInvalidateCacheAfterLoading()
    {
        // Arrange
        var command = new LoadHolidaysCommand(2024, "GB");
        var expectedResult = new Response<LoadHolidaysResult>(true, new LoadHolidaysResult(1), "Successfully loaded 1 holidays");
        
        _cacheInvalidationDecorator.HandleAsync(Arg.Any<LoadHolidaysCommand>(), Arg.Any<Func<LoadHolidaysCommand, CancellationToken, Task<Response<LoadHolidaysResult>>>>(), Arg.Any<CancellationToken>())
            .Returns(expectedResult);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Status.Should().BeTrue();
        
        await _cacheInvalidationDecorator.Received(1)
            .HandleAsync(Arg.Any<LoadHolidaysCommand>(), Arg.Any<Func<LoadHolidaysCommand, CancellationToken, Task<Response<LoadHolidaysResult>>>>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task HandleCommandAsyncShouldHandleEmptyHolidayList()
    {
        // Arrange
        var command = new LoadHolidaysCommand(2024, "US");
        var expectedResult = new Response<LoadHolidaysResult>(true, new LoadHolidaysResult(0), "Successfully loaded 0 holidays");
        
        _cacheInvalidationDecorator.HandleAsync(Arg.Any<LoadHolidaysCommand>(), Arg.Any<Func<LoadHolidaysCommand, CancellationToken, Task<Response<LoadHolidaysResult>>>>(), Arg.Any<CancellationToken>())
            .Returns(expectedResult);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Status.Should().BeTrue();
        result.Data.HolidaysLoaded.Should().Be(0);
        result.Message.Should().Contain("Successfully loaded 0 holidays");
        
        await _cacheInvalidationDecorator.Received(1)
            .HandleAsync(Arg.Any<LoadHolidaysCommand>(), Arg.Any<Func<LoadHolidaysCommand, CancellationToken, Task<Response<LoadHolidaysResult>>>>(), Arg.Any<CancellationToken>());
    }
}
