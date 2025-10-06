namespace HolidayApp.Domain.Entities;

public sealed class Holiday
{
    public int Id { get; set; }
    public int CountryId { get; set; }
    public DateTime Date { get; set; }
    public string LocalName { get; set; } = string.Empty;
    public string GlobalName { get; set; } = string.Empty;
    public bool IsWeekend { get; set; }
    public int Year { get; set; }
    public Country Country { get; set; } = null!;
}