namespace HolidayApp.UnitTests.TestData;

public static class ValidationTestData
{
    private static class Years
    {
        public static int Valid => DateTime.Now.Year;
        public static int TooOld => 1900;
        public static int TooFarInFuture => DateTime.Now.Year + 11;
        public static int MinValid => 1901;
        public static int MaxValid => DateTime.Now.Year + 10;
    }

    private static class CountryCodes
    {
        public static string ValidUS => "US";
        public static string ValidGB => "GB";
        public static string ValidCA => "CA";
        public static string ValidDE => "DE";
        public static string Empty => "";
        public static string TooShort => "U";
        public static string TooLong => "USA";
        public static string Lowercase => "us";
        public static string Mixed => "Us";
        public static string WithNumbers => "U1";
        public static string WithSpecialChars => "U$";
    }

    public static IEnumerable<object[]> InvalidYearData()
    {
        yield return [Years.TooOld, "Year must be greater than 1900"];
        yield return [Years.TooFarInFuture, "Year must not be more than 10 years in the future"];
        yield return [1899, "Year must be greater than 1900"];
        yield return [0, "Year must be greater than 1900"];
    }

    public static IEnumerable<object[]> ValidYearData()
    {
        yield return [Years.MinValid];
        yield return [Years.Valid];
        yield return [Years.MaxValid];
    }

    public static IEnumerable<object[]> InvalidCountryCodeData()
    {
        yield return [CountryCodes.Empty, "Country code is required"];
        yield return [CountryCodes.TooShort, "Country code must be exactly 2 characters"];
        yield return [CountryCodes.TooLong, "Country code must be exactly 2 characters"];
        yield return [CountryCodes.Lowercase, "Country code must be 2 uppercase letters"];
        yield return [CountryCodes.Mixed, "Country code must be 2 uppercase letters"];
        yield return [CountryCodes.WithNumbers, "Country code must be 2 uppercase letters"];
        yield return [CountryCodes.WithSpecialChars, "Country code must be 2 uppercase letters"];
    }

    public static IEnumerable<object[]> ValidCountryCodeData()
    {
        yield return [CountryCodes.ValidUS];
        yield return [CountryCodes.ValidGB];
        yield return [CountryCodes.ValidCA];
        yield return [CountryCodes.ValidDE];
    }
}
