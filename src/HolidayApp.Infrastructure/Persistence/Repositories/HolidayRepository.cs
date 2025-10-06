using HolidayApp.Application.Common.Interfaces;
using HolidayApp.Application.Common.Models.Dtos;
using HolidayApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HolidayApp.Infrastructure.Persistence.Repositories;

   public class HolidayRepository(ApplicationDbContext context, ILogger<HolidayRepository> logger) : IHolidayRepository
   {
       public async Task<List<LastCelebratedHolidayDto>> GetLastCelebratedHolidaysAsync(string countryCode, int count,
           CancellationToken cancellationToken)
        {
            var today = DateTime.Today;

            return await context.Holidays
                .AsNoTracking()
                .Where(h => h.Country.CountryCode == countryCode && h.Date < today)
                .OrderByDescending(h => h.Date)
                .Take(count)
                .Select(h => new LastCelebratedHolidayDto
                {
                    Date = h.Date,
                    Name = h.LocalName
                })
                .ToListAsync(cancellationToken);
        }

        public async Task<List<CountryHolidayCountDto>> GetWeekdayHolidayCountsAsync(
            int year, string[] countryCodes, CancellationToken cancellationToken)
        {
            return await context.Holidays
                .AsNoTracking()
                .Where(h => h.Year == year && countryCodes.Contains(h.Country.CountryCode))
                .Where(h => !h.IsWeekend)
                .GroupBy(h => h.Country.CountryCode)
                .Select(g => new CountryHolidayCountDto
                {
                    CountryCode = g.Key,
                    HolidayCount = g.Count()
                })
                .OrderByDescending(x => x.HolidayCount)
                .ToListAsync(cancellationToken);
        }

        public async Task<List<CommonHolidayDto>> GetCommonHolidaysAsync(
            int year, string countryCode1, string countryCode2, CancellationToken cancellationToken)
        {
            var commonHolidays = await context.Holidays
                .AsNoTracking()
                .Where(h1 => h1.Year == year && h1.Country.CountryCode == countryCode1)
                .Join(
                    context.Holidays.Where(h2 => h2.Year == year && h2.Country.CountryCode == countryCode2),
                    h1 => h1.Date,
                    h2 => h2.Date,
                    (h1, h2) => new CommonHolidayDto
                    {
                        Date = h1.Date,
                        LocalName1 = h1.LocalName,
                        LocalName2 = h2.LocalName
                    })
                .OrderBy(c => c.Date)
                .ToListAsync(cancellationToken);

            logger.LogInformation("Found {Count} common holidays between {Country1} and {Country2} for year {Year}",
                commonHolidays.Count, countryCode1, countryCode2, year);

            return commonHolidays;
        }

        public async Task BulkInsertHolidaysAsync(List<Holiday> holidays, CancellationToken cancellationToken)
        {
            if (holidays.Count == 0) return;

            await using var transaction = await context.Database.BeginTransactionAsync(cancellationToken);
            try
            {
                await context.BulkInsertAsync(holidays, cancellationToken);
                await transaction.CommitAsync(cancellationToken);
                logger.LogInformation("Successfully bulk inserted {Count} holidays", holidays.Count);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to bulk insert {Count} holidays. Rolling back transaction.", holidays.Count);
                await transaction.RollbackAsync(cancellationToken);
                throw;
            }
        }
        
        public async Task<bool> HolidaysExistForYearAsync(string countryCode, int year, CancellationToken cancellationToken)
        {
            return await context.Holidays
                .AsNoTracking()
                .AnyAsync(h => h.Country.CountryCode == countryCode && h.Year == year, cancellationToken);
        }
    }