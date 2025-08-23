using EmployeeProjectApi.Dtos;
using FluentValidation;

public class ProjectValidator : AbstractValidator<ProjectDto>
{
    public ProjectValidator()
    {
        RuleFor(p => p.Name).NotEmpty().MaximumLength(100);
        RuleFor(p => p.StartDate).LessThan(p => p.EndDate);
        RuleFor(p => p.EndDate).GreaterThan(p => p.StartDate);
        RuleFor(p => p.Status).NotEmpty();
    }
}
