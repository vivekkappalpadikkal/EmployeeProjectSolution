using EmployeeProjectApi.Dtos;
using FluentValidation;

public class EmployeeValidator : AbstractValidator<EmployeeDto>
{
    public EmployeeValidator()
    {
        RuleFor(e => e.Name).NotEmpty().MaximumLength(100);
        RuleFor(e => e.Email).EmailAddress();
        RuleFor(e => e.Department).NotEmpty();
        RuleFor(e => e.DateOfJoining)
            .LessThanOrEqualTo(DateTime.Today)
            .WithMessage("DateOfJoining cannot be in the future");
    }
}
