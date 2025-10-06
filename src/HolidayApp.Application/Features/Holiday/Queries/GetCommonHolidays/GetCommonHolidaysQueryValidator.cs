using FluentValidation;

namespace HolidayApp.Application.Features.Holiday.Queries.GetCommonHolidays;

public class GetCommonHolidaysQueryValidator : AbstractValidator<GetCommonHolidaysQuery>
{
    public GetCommonHolidaysQueryValidator()
    {
        RuleFor(x => x.Year)
            .GreaterThan(1900)
            .WithMessage("Year must be greater than 1900")
            .LessThanOrEqualTo(DateTime.Now.Year + 10)
            .WithMessage($"Year must not be more than 10 years in the future");

        RuleFor(x => x.CountryCode1)
            .NotEmpty()
            .WithMessage("Country code 1 is required")
            .Length(2)
            .WithMessage("Country code 1 must be exactly 2 characters")
            .Matches("^[A-Z]{2}$")
            .WithMessage("Country code 1 must be 2 uppercase letters");

        RuleFor(x => x.CountryCode2)
            .NotEmpty()
            .WithMessage("Country code 2 is required")
            .Length(2)
            .WithMessage("Country code 2 must be exactly 2 characters")
            .Matches("^[A-Z]{2}$")
            .WithMessage("Country code 2 must be 2 uppercase letters");

        RuleFor(x => x)
            .Must(x => x.CountryCode1 != x.CountryCode2)
            .WithMessage("Country codes must be different")
            .When(x => !string.IsNullOrEmpty(x.CountryCode1) && !string.IsNullOrEmpty(x.CountryCode2));
    }
}