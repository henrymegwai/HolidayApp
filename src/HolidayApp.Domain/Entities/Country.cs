namespace HolidayApp.Domain.Entities;

public sealed class Country
{
    public int Id { get; set; }
    public string CountryCode { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public ICollection<Holiday> Holidays { get; set; } = new List<Holiday>();
}