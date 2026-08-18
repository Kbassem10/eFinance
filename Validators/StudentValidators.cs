using FluentValidation;
using StudentRegistrationPortal.Api.DTOs;

namespace StudentRegistrationPortal.Api.Validators;

public class CreateStudentDtoValidator : AbstractValidator<CreateStudentDto>
{
    public CreateStudentDtoValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("A valid email address is required.")
            .MaximumLength(255).WithMessage("Email cannot exceed 255 characters.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required.")
            .MinimumLength(6).WithMessage("Password must be at least 6 characters.")
            .MaximumLength(100).WithMessage("Password cannot exceed 100 characters.");

        RuleFor(x => x.DepartmentId)
            .GreaterThan(0)
            .WithMessage("A valid Department ID must be greater than 0.");

        RuleFor(x => x.StudentStatusId)
            .GreaterThan(0)
            .WithMessage("A valid Status ID must be greater than 0.");

        RuleFor(x => x.StudentNumber)
            .NotEmpty().WithMessage("Student Number is required.")
            .Length(3, 30).WithMessage("Student Number must be between 3 and 30 characters.");

        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("First Name is required.")
            .Length(2, 100).WithMessage("First Name must be between 2 and 100 characters.");

        RuleFor(x => x.MiddleName)
            .MaximumLength(100).WithMessage("Middle Name cannot exceed 100 characters.")
            .When(x => !string.IsNullOrWhiteSpace(x.MiddleName));

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("Last Name is required.")
            .Length(2, 100).WithMessage("Last Name must be between 2 and 100 characters.");

        RuleFor(x => x.NationalId)
            .MaximumLength(30).WithMessage("National ID cannot exceed 30 characters.")
            .When(x => !string.IsNullOrWhiteSpace(x.NationalId));

        RuleFor(x => x.DateOfBirth)
            .NotEmpty().WithMessage("Date of Birth is required.")
            .LessThan(DateOnly.FromDateTime(DateTime.UtcNow)).WithMessage("Date of Birth must be in the past.");

        RuleFor(x => x.Gender)
            .MaximumLength(20).WithMessage("Gender cannot exceed 20 characters.")
            .When(x => !string.IsNullOrWhiteSpace(x.Gender));

        RuleFor(x => x.PhoneNumber)
            .MaximumLength(30).WithMessage("Phone Number cannot exceed 30 characters.")
            .Matches(@"^\+?[0-9\s\-()]{7,30}$").WithMessage("Invalid phone number format.")
            .When(x => !string.IsNullOrWhiteSpace(x.PhoneNumber));

        RuleFor(x => x.Address)
            .MaximumLength(255).WithMessage("Address cannot exceed 255 characters.")
            .When(x => !string.IsNullOrWhiteSpace(x.Address));

        RuleFor(x => x.AdmissionDate)
            .NotEmpty().WithMessage("Admission Date is required.");
    }
}

public class UpdateStudentDtoValidator : AbstractValidator<UpdateStudentDto>
{
    public UpdateStudentDtoValidator()
    {
        RuleFor(x => x.DepartmentId)
            .GreaterThan(0)
            .WithMessage("A valid Department ID must be greater than 0.");

        RuleFor(x => x.StudentStatusId)
            .GreaterThan(0)
            .WithMessage("A valid Status ID must be greater than 0.");

        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("First Name is required.")
            .Length(2, 100).WithMessage("First Name must be between 2 and 100 characters.");

        RuleFor(x => x.MiddleName)
            .MaximumLength(100).WithMessage("Middle Name cannot exceed 100 characters.")
            .When(x => !string.IsNullOrWhiteSpace(x.MiddleName));

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("Last Name is required.")
            .Length(2, 100).WithMessage("Last Name must be between 2 and 100 characters.");

        RuleFor(x => x.PhoneNumber)
            .MaximumLength(30).WithMessage("Phone Number cannot exceed 30 characters.")
            .Matches(@"^\+?[0-9\s\-()]{7,30}$").WithMessage("Invalid phone number format.")
            .When(x => !string.IsNullOrWhiteSpace(x.PhoneNumber));

        RuleFor(x => x.Address)
            .MaximumLength(255).WithMessage("Address cannot exceed 255 characters.")
            .When(x => !string.IsNullOrWhiteSpace(x.Address));

        RuleFor(x => x.AdmissionDate)
            .NotEmpty().WithMessage("Admission Date is required.");
    }
}
