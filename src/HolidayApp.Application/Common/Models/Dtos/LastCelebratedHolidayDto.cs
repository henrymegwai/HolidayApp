namespace HolidayApp.Application.Common.Models.Dtos;

public record LastCelebratedHolidayDto
{
    public DateTime Date { get; set; }
    public string Name { get; set; } = string.Empty;
}