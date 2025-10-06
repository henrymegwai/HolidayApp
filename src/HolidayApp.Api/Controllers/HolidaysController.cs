using HolidayApp.Api.Requests;
using HolidayApp.Application.Common.Models.Dtos;
using HolidayApp.Application.Features.Country.Commands;
using HolidayApp.Application.Features.Holiday.Commands;
using HolidayApp.Application.Features.Holiday.Queries.GetCommonHolidays;
using HolidayApp.Application.Features.Holiday.Queries.GetLastCelebratedHolidays;
using HolidayApp.Application.Features.Holiday.Queries.GetWeekdayHolidayCount;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace HolidayApp.Api.Controllers;

[ApiController]
[Route("api/holidays")]
public class HolidaysController(IMediator mediator) : ControllerBase
{
    /// <summary>
    /// Load all available countries from Nager.Date Api into the database
    /// </summary>
    [HttpPost("load-countries")]
    [ProducesResponseType(typeof(LoadCountriesResult), StatusCodes.Status200OK)]
    public async Task<ActionResult<LoadCountriesResult>> LoadCountries(CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new LoadCountriesCommand(), cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Load holidays for a specific year and country into the database
    /// </summary>
    [HttpPost("load-holidays")]
    [ProducesResponseType(typeof(LoadHolidaysResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<LoadHolidaysResult>> LoadHolidays([FromBody] LoadHolidaysRequest request,
        CancellationToken cancellationToken)
    {
        var command = new LoadHolidaysCommand(request.Year, request.CountryCode.ToUpper());

        var result = await mediator.Send(command, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Get last 3 celebrated holidays for a country from the database
    /// </summary>
    [HttpGet("last-celebrated/{countryCode}/{numberOfHolidays:int}")]
    [ProducesResponseType(typeof(List<LastCelebratedHolidayDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<List<LastCelebratedHolidayDto>>> GetLastCelebratedHolidays(
        [FromRoute] string countryCode,
        [FromRoute] int numberOfHolidays,
        CancellationToken cancellationToken)
    {
        var query = new GetLastCelebratedHolidaysQuery(countryCode.ToUpper(), numberOfHolidays);

        var result = await mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Get number of weekday holidays per country for a given year (sorted descending)
    /// </summary>
    [HttpGet("weekday-count")]
    [ProducesResponseType(typeof(List<CountryHolidayCountDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<List<CountryHolidayCountDto>>> GetWeekdayHolidayCount(
        [FromQuery] int year,
        [FromQuery] string[] countryCodes,
        CancellationToken cancellationToken)
    {
        var query = new GetWeekdayHolidayCountQuery(
            year,
            countryCodes.Select(c => c.ToUpper()).ToArray());

        var result = await mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Get common holidays celebrated in both countries for a given year
    /// </summary>
    [HttpGet("common")]
    [ProducesResponseType(typeof(List<CommonHolidayDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<List<CommonHolidayDto>>> GetCommonHolidays(
        [FromQuery] int year,
        [FromQuery] string countryCode1,
        [FromQuery] string countryCode2,
        CancellationToken cancellationToken)
    {
        var query = new GetCommonHolidaysQuery(
            year,
            countryCode1.ToUpper(),
            countryCode2.ToUpper());

        var result = await mediator.Send(query, cancellationToken);
        return Ok(result);
    }
}