using FluentValidation;
using StudentRegistrationPortal.Api.DTOs;

namespace StudentRegistrationPortal.Api.Validators;

public class CreateCourseDtoValidator : AbstractValidator<CreateCourseDto>
{
    public CreateCourseDtoValidator()
    {
        RuleFor(x => x.CourseCode)
            .NotEmpty().WithMessage("Course Code is required.")
            .MaximumLength(20).WithMessage("Course Code cannot exceed 20 characters.");

        RuleFor(x => x.CourseName)
            .NotEmpty().WithMessage("Course Name is required.")
            .MaximumLength(150).WithMessage("Course Name cannot exceed 150 characters.");

        RuleFor(x => x.CreditHours)
            .InclusiveBetween(1, 10).WithMessage("Credit Hours must be between 1 and 10.");

        RuleFor(x => x.DifficultyLevel)
            .NotEmpty().WithMessage("Difficulty Level is required.")
            .MaximumLength(50).WithMessage("Difficulty Level cannot exceed 50 characters.");

        RuleFor(x => x.CourseStatusId)
            .GreaterThan(0).WithMessage("A valid Course Status ID is required.");
    }
}

public class UpdateCourseDtoValidator : AbstractValidator<UpdateCourseDto>
{
    public UpdateCourseDtoValidator()
    {
        RuleFor(x => x.CourseName)
            .NotEmpty().WithMessage("Course Name is required.")
            .MaximumLength(150).WithMessage("Course Name cannot exceed 150 characters.");

        RuleFor(x => x.CreditHours)
            .InclusiveBetween(1, 10).WithMessage("Credit Hours must be between 1 and 10.");

        RuleFor(x => x.DifficultyLevel)
            .NotEmpty().WithMessage("Difficulty Level is required.")
            .MaximumLength(50).WithMessage("Difficulty Level cannot exceed 50 characters.");

        RuleFor(x => x.CourseStatusId)
            .GreaterThan(0).WithMessage("A valid Course Status ID is required.");
    }
}

