using FluentAssertions;
using HolidayApp.Domain.Entities;
using HolidayApp.Infrastructure.Persistence;
using HolidayApp.Infrastructure.Persistence.Repositories;
using HolidayApp.UnitTests.TestData;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace HolidayApp.UnitTests.Infrastructure.Repositories;

public class HolidayRepositoryTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly HolidayRepository _repository;

    public HolidayRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
            .Options;

        _context = new ApplicationDbContext(options);
        var logger = Substitute.For<ILogger<HolidayRepository>>();
        _repository = new HolidayRepository(_context, logger);
    }

    [Fact]
    public async Task GetLastCelebratedHolidaysAsyncShouldReturnLastThreeHolidays()
    {
        // Arrange
        var country = CountryTestDataFactory.CreateUnitedStates();
        _context.Countries.Add(country);

        var today = DateTime.Today;
        var holidays = new List<Holiday>
        {
            HolidayTestDataFactory.CreateHoliday(1, 1, today.AddDays(-10), "Holiday 1", "Holiday 1"),
            HolidayTestDataFactory.CreateHoliday(2, 1, today.AddDays(-5), "Holiday 2", "Holiday 2"),
            HolidayTestDataFactory.CreateHoliday(3, 1, today.AddDays(-2), "Holiday 3", "Holiday 3"),
            HolidayTestDataFactory.CreateHoliday(4, 1, today.AddDays(5), "Future Holiday", "Future")
        };
        _context.Holidays.AddRange(holidays);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetLastCelebratedHolidaysAsync("US", 3, CancellationToken.None);

        // Assert
        result.Should().HaveCount(3);
        result.Should().NotContain(h => h.Name == "Future Holiday");
        result.First().Name.Should().Be("Holiday 3");
    }

    [Fact]
    public async Task GetWeekdayHolidayCountsAsyncShouldReturnCountsInDescendingOrder()
    {
        // Arrange
        var us = CountryTestDataFactory.CreateUnitedStates();
        var gb = CountryTestDataFactory.CreateUnitedKingdom();
        _context.Countries.AddRange(us, gb);

        // US: 3 weekday holidays
        _context.Holidays.AddRange(
            HolidayTestDataFactory.CreateNewYearHoliday(1),
            HolidayTestDataFactory.CreateIndependenceDayHoliday(1),
            HolidayTestDataFactory.CreateChristmasHoliday(1),
            HolidayTestDataFactory.CreateWeekendHoliday(1, new DateTime(2024, 12, 28), "Weekend Holiday")
        );

        // GB: 5 weekday holidays
        _context.Holidays.AddRange(
            HolidayTestDataFactory.CreateNewYearHoliday(2),
            HolidayTestDataFactory.CreateHoliday(0, 2, new DateTime(2024, 3, 29), "Good Friday", "Good Friday"),
            HolidayTestDataFactory.CreateHoliday(0, 2, new DateTime(2024, 5, 6), "May Day", "May Day"),
            HolidayTestDataFactory.CreateChristmasHoliday(2),
            HolidayTestDataFactory.CreateBoxingDayHoliday(2)
        );

        await _context.SaveChangesAsync();

        // Act
        var result 
            = await _repository.GetWeekdayHolidayCountsAsync(2024, ["US", "GB"], CancellationToken.None);

        // Assert
        result.Should().HaveCount(2);
        result.First().CountryCode.Should().Be("GB");
        result.First().HolidayCount.Should().Be(5);
        result.Last().CountryCode.Should().Be("US");
        result.Last().HolidayCount.Should().Be(3);
    }

    [Fact]
    public async Task GetCommonHolidaysAsyncShouldReturnOnlyCommonDates()
    {
        // Arrange
        var us = CountryTestDataFactory.CreateUnitedStates();
        var ca = CountryTestDataFactory.CreateCanada();
        _context.Countries.AddRange(us, ca);
        await _context.SaveChangesAsync();

        var commonDate = new DateTime(2024, 1, 1);
        _context.Holidays.AddRange(
           
            HolidayTestDataFactory.CreateHoliday(0, us.Id, commonDate, "New Year's Day", "New Year"),
            HolidayTestDataFactory.CreateHoliday(0, ca.Id, commonDate, "Jour de l'An", "New Year"),
            HolidayTestDataFactory.CreateHoliday(0, us.Id, new DateTime(2024, 7, 4), "Independence Day", "Independence Day"),
            HolidayTestDataFactory.CreateHoliday(0, ca.Id, new DateTime(2024, 7, 1), "Canada Day", "Canada Day")
        );

        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetCommonHolidaysAsync(2024, "US", "CA", CancellationToken.None);

        // Assert
        result.Should().HaveCount(1);
        result.First().Date.Should().Be(commonDate);
        result.First().LocalName1.Should().Be("New Year's Day");
        result.First().LocalName2.Should().Be("Jour de l'An");
    }

    [Fact]
    public async Task HolidaysExistForYearAsyncShouldReturnTrueWhenHolidaysExist()
    {
        // Arrange
        var country = CountryTestDataFactory.CreateUnitedStates();
        _context.Countries.Add(country);
        _context.Holidays.Add(HolidayTestDataFactory.CreateNewYearHoliday(1));
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.HolidaysExistForYearAsync("US", 2024, CancellationToken.None);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task HolidaysExistForYearAsyncShouldReturnFalseWhenNoHolidays()
    {
        // Arrange
        var country = CountryTestDataFactory.CreateUnitedStates();
        _context.Countries.Add(country);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.HolidaysExistForYearAsync("US", 2024, CancellationToken.None);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task BulkInsertHolidaysAsyncShouldHandleEmptyList()
    {
        // Arrange
        var emptyHolidays = new List<Holiday>();

        // Act
        await _repository.BulkInsertHolidaysAsync(emptyHolidays, CancellationToken.None);

        // Assert
        var savedHolidays = await _context.Holidays.ToListAsync();
        savedHolidays.Should().BeEmpty();
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }
}
