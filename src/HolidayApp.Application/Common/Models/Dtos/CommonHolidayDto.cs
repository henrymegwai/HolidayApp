namespace HolidayApp.Application.Common.Models.Dtos;

public record CommonHolidayDto
{
    public DateTime Date { get; set; }
    public string LocalName1 { get; set; } = string.Empty;
    public string LocalName2 { get; set; } = string.Empty;
}