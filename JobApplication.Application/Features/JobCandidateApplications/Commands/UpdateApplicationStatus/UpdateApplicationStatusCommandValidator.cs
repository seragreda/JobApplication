using FluentValidation;

namespace JobApplication.Application.Features.JobCandidateApplications.Commands.UpdateApplicationStatus;

public class UpdateApplicationStatusCommandValidator : AbstractValidator<UpdateApplicationStatusCommand>
{
    public UpdateApplicationStatusCommandValidator()
    {
        RuleFor(x => x.ApplicationId).GreaterThan(0);
        RuleFor(x => x.NewStatus).IsInEnum();
    }
}