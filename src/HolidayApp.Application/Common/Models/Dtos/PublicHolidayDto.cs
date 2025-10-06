namespace HolidayApp.Application.Common.Models.Dtos;

public record PublicHolidayDto
{
    public DateTime Date { get; set; }
    public string LocalName { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string CountryCode { get; set; } = string.Empty;
}