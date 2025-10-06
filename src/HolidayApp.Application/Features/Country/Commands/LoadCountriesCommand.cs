using HolidayApp.Application.Common.Interfaces;
using HolidayApp.Application.Common.Models;
using HolidayApp.Application.Common.Models.Dtos;

namespace HolidayApp.Application.Features.Country.Commands;

public record LoadCountriesCommand : ICommand<Response<LoadCountriesResult>>;

public class LoadCountriesResult
{
    public int CountriesLoaded { get; set; }
    public List<CountryInfoDto> Countries { get; set; } = [];
}