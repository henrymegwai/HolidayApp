using FluentValidation;

namespace HolidayApp.Application.Features.Holiday.Queries.GetWeekdayHolidayCount;

public class GetWeekdayHolidayCountQueryValidator : AbstractValidator<GetWeekdayHolidayCountQuery>
{
    public GetWeekdayHolidayCountQueryValidator()
    {
        RuleFor(x => x.Year)
            .GreaterThan(1900)
            .WithMessage("Year must be greater than 1900")
            .LessThanOrEqualTo(DateTime.Now.Year + 10)
            .WithMessage($"Year must not be more than 10 years in the future");

        RuleFor(x => x.CountryCodes)
            .NotEmpty()
            .WithMessage("At least one country code is required");

        RuleForEach(x => x.CountryCodes)
            .NotEmpty()
            .WithMessage("Country code cannot be empty")
            .Length(2)
            .WithMessage("Each country code must be exactly 2 characters")
            .Matches("^[A-Z]{2}$")
            .WithMessage("Each country code must be 2 uppercase letters");
    }
}
