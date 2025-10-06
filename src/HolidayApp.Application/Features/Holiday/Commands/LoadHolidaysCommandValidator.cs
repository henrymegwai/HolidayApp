using FluentValidation;

namespace HolidayApp.Application.Features.Holiday.Commands;

public class LoadHolidaysCommandValidator : AbstractValidator<LoadHolidaysCommand>
{
    public LoadHolidaysCommandValidator()
    {
        RuleFor(x => x.Year)
            .GreaterThan(1900)
            .WithMessage("Year must be greater than 1900")
            .LessThanOrEqualTo(DateTime.Now.Year + 10)
            .WithMessage($"Year must not be more than 10 years in the future");

        RuleFor(x => x.CountryCode)
            .NotEmpty()
            .WithMessage("Country code is required")
            .Length(2)
            .WithMessage("Country code must be exactly 2 characters")
            .Matches("^[A-Z]{2}$")
            .WithMessage("Country code must be 2 uppercase letters");
    }
}