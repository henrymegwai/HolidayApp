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

public class CountryRepositoryTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly CountryRepository _repository;

    public CountryRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
            .Options;

        _context = new ApplicationDbContext(options);
        var logger = Substitute.For<ILogger<CountryRepository>>();
        _repository = new CountryRepository(_context, logger);
    }

    [Fact]
    public async Task GetByCodeAsyncShouldReturnCountryWhenExists()
    {
        // Arrange
        var country = CountryTestDataFactory.CreateUnitedStates();
        _context.Countries.Add(country);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByCodeAsync("US", CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.CountryCode.Should().Be("US");
        result.Name.Should().Be("United States");
    }

    [Fact]
    public async Task GetByCodeAsyncShouldReturnNullWhenCountryDoesNotExist()
    {
        // Act
        var result = await _repository.GetByCodeAsync("XX", CancellationToken.None);

        // Assert
        result.Should().BeNull();
    }

    [Theory]
    [InlineData("US")]
    [InlineData("GB")]
    [InlineData("CA")]
    public async Task GetByCodeAsyncShouldReturnCorrectCountryForDifferentCodes(string countryCode)
    {
        // Arrange
        var countries = CountryTestDataFactory.CreateMultipleCountries();
        _context.Countries.AddRange(countries);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByCodeAsync(countryCode, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.CountryCode.Should().Be(countryCode);
    }

    [Fact]
    public async Task GetAllAsyncShouldReturnAllCountriesOrderedByName()
    {
        // Arrange
        var countries = new List<Country>
        {
            CountryTestDataFactory.CreateGermany(),     
            CountryTestDataFactory.CreateUnitedStates(),
            CountryTestDataFactory.CreateCanada(),
            CountryTestDataFactory.CreateFrance() 
        };
        _context.Countries.AddRange(countries);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetAllAsync(CancellationToken.None);

        // Assert
        result.Should().HaveCount(4);
        result.Should().BeInAscendingOrder(c => c.Name);
        result[0].Name.Should().Be("Canada");
        result[1].Name.Should().Be("France");
        result[2].Name.Should().Be("Germany");
        result[3].Name.Should().Be("United States");
    }

    [Fact]
    public async Task GetAllAsyncShouldReturnEmptyListWhenNoCountries()
    {
        // Act
        var result = await _repository.GetAllAsync(CancellationToken.None);

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetAllAsyncShouldNotIncludeIdInProjection()
    {
        // Arrange
        var country = CountryTestDataFactory.CreateUnitedStates();
        _context.Countries.Add(country);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetAllAsync(CancellationToken.None);

        // Assert
        result.Should().HaveCount(1);
        result[0].Id.Should().Be(0);
        result[0].CountryCode.Should().Be("US");
        result[0].Name.Should().Be("United States");
    }

    [Fact]
    public async Task CountryExistsAsyncShouldReturnTrueWhenCountryExists()
    {
        // Arrange
        var country = CountryTestDataFactory.CreateUnitedKingdom();
        _context.Countries.Add(country);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.CountryExistsAsync("GB");

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task CountryExistsAsyncShouldReturnFalseWhenCountryDoesNotExist()
    {
        // Act
        var result = await _repository.CountryExistsAsync("XX");

        // Assert
        result.Should().BeFalse();
    }

    [Theory]
    [InlineData("US", true)]
    [InlineData("GB", true)]
    [InlineData("XX", false)]
    [InlineData("YY", false)]
    public async Task CountryExistsAsyncShouldReturnCorrectResultForMultipleScenarios(string countryCode, bool expectedExists)
    {
        // Arrange
        var countries = new List<Country>
        {
            CountryTestDataFactory.CreateUnitedStates(),
            CountryTestDataFactory.CreateUnitedKingdom()
        };
        _context.Countries.AddRange(countries);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.CountryExistsAsync(countryCode);

        // Assert
        result.Should().Be(expectedExists);
    }

    [Fact]
    public async Task AddAsyncShouldAddCountryToDatabase()
    {
        // Arrange
        var country = CountryTestDataFactory.CreateCanada();

        // Act
        await _repository.AddAsync(country, CancellationToken.None);

        // Assert
        var savedCountry = await _context.Countries.FirstOrDefaultAsync(c => c.CountryCode == "CA");
        savedCountry.Should().NotBeNull();
        savedCountry.Name.Should().Be("Canada");
    }

    [Fact]
    public async Task AddAsyncShouldPersistChangesToDatabase()
    {
        // Arrange
        var country = CountryTestDataFactory.CreateGermany();

        // Act
        await _repository.AddAsync(country, CancellationToken.None);

        // Assert
        var count = await _context.Countries.CountAsync();
        count.Should().Be(1);
    }

    [Fact]
    public async Task AddRangeAsyncShouldHandleEmptyList()
    {
        // Arrange
        var emptyCountries = new List<Country>();

        // Act
        await _repository.AddRangeAsync(emptyCountries, CancellationToken.None);

        // Assert
        var count = await _context.Countries.CountAsync();
        count.Should().Be(0);
    }
    
    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }
}
