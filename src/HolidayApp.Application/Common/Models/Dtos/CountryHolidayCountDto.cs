namespace HolidayApp.Application.Common.Models.Dtos;

public record CountryHolidayCountDto
{
    public string CountryCode { get; set; } = string.Empty;
    public int HolidayCount { get; set; }
}