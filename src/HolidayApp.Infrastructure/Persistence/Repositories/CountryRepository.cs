using HolidayApp.Application.Common.Interfaces;
using HolidayApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HolidayApp.Infrastructure.Persistence.Repositories;

public class CountryRepository(ApplicationDbContext context, ILogger<CountryRepository> logger) : ICountryRepository
{
    public async Task<Country?> GetByCodeAsync(string countryCode, CancellationToken cancellationToken)
    {
        return await context.Countries
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.CountryCode == countryCode, cancellationToken);
    }

    public async Task<List<Country>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await context.Countries
            .AsNoTracking()
            .OrderBy(c => c.Name)
            .Select(x => new Country
            {
                CountryCode = x.CountryCode,
                Name = x.Name
            })
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> CountryExistsAsync(string countryCode)
    {
        return await context.Countries
            .AsNoTracking()
            .AnyAsync(c => c.CountryCode == countryCode);
    }

    public async Task AddAsync(Country country,  CancellationToken cancellationToken)
    {
        await context.Countries.AddAsync(country, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task AddRangeAsync(List<Country> countries, CancellationToken cancellationToken)
    {
        if (countries.Count == 0) return;

        await using var transaction = await context.Database.BeginTransactionAsync(cancellationToken);
        try
        {
            await context.BulkInsertAsync(countries, cancellationToken);
            await transaction.CommitAsync(cancellationToken);
            logger.LogInformation("Successfully bulk inserted {Count} countries", countries.Count);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to bulk insert {Count} countries. Rolling back transaction.", countries.Count);
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }
    
}