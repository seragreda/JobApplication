using FluentValidation;

namespace JobApplication.Application.Features.Auth.Commands.RegisterRecruiter;

public class RegisterRecruiterCommandValidator : AbstractValidator<RegisterRecruiterCommand>
{
    public RegisterRecruiterCommandValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.Password).NotEmpty().MinimumLength(6);
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
        RuleFor(x => x.CompanyName).NotEmpty().MaximumLength(200);
        RuleFor(x => x.InvitationCode).NotEmpty();
    }
}