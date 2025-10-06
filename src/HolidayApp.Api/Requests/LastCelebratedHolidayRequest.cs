namespace HolidayApp.Api.Requests;

public class LastCelebratedHolidayRequest
{
    public int Year { get; set; }
    public string CountryCode1 { get; set; } = string.Empty;
    public string CountryCode2 { get; set; } = string.Empty;
}