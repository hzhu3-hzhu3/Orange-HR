using FluentValidation;
using HRManagement.Application.DTOs;
namespace HRManagement.Application.Validators;
public class UpdateEmployeeValidator : AbstractValidator<UpdateEmployeeRequest>
{
    public UpdateEmployeeValidator()
    {
        RuleFor(x => x.Name)
            .MaximumLength(200)
            .WithMessage("Name must not exceed 200 characters.")
            .When(x => x.Name != null);
        RuleFor(x => x.Email)
            .EmailAddress()
            .WithMessage("Email must be a valid email address.")
            .MaximumLength(200)
            .WithMessage("Email must not exceed 200 characters.")
            .When(x => x.Email != null);
        RuleFor(x => x.Role)
            .IsInEnum()
            .WithMessage("Role must be a valid role value.")
            .When(x => x.Role != null);
    }
}
