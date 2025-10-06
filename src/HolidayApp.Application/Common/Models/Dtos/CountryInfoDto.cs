namespace HolidayApp.Application.Common.Models.Dtos;

public record CountryInfoDto
{
    public string CountryCode { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
}