using HolidayApp.Application.Common.Models.Dtos;
using HolidayApp.Domain.Entities;

namespace HolidayApp.Application.Common.Mapper;

public static class Mapper
{
    public static Country Map(this CountryInfoDto countryInfoDto)
    {
        return new Country
        {
            CountryCode = countryInfoDto.CountryCode,
            Name = countryInfoDto.Name
        };
    }
    
    public static CountryInfoDto MapToDto(this Country country)
    {
        return new CountryInfoDto
        {
            CountryCode = country.CountryCode,
            Name = country.Name
        };
    }

    public static Holiday Map(this PublicHolidayDto publicHoliday, int countryId, int year)
    {
        return new Holiday
        {
            CountryId = countryId,
            Date = publicHoliday.Date,
            LocalName = publicHoliday.LocalName,
            GlobalName = publicHoliday.Name,
            Year = year,
            IsWeekend = publicHoliday.Date.DayOfWeek == DayOfWeek.Saturday || publicHoliday.Date.DayOfWeek == DayOfWeek.Sunday
        };
    }
}