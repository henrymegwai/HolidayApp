namespace HolidayApp.Api.Requests;

public class LoadHolidaysRequest
{
    public int Year { get; set; }
    public string CountryCode { get; set; } = string.Empty;
}