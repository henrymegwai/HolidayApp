using FluentValidation;

namespace HolidayApp.Application.Features.Holiday.Queries.GetLastCelebratedHolidays;

public class GetLastCelebratedHolidaysQueryValidator : AbstractValidator<GetLastCelebratedHolidaysQuery>
{
    public GetLastCelebratedHolidaysQueryValidator()
    {
        RuleFor(x => x.CountryCode)
            .NotEmpty()
            .WithMessage("Country code is required")
            .Length(2)
            .WithMessage("Country code must be exactly 2 characters")
            .Matches("^[A-Z]{2}$")
            .WithMessage("Country code must be 2 uppercase letters");
    }
}
