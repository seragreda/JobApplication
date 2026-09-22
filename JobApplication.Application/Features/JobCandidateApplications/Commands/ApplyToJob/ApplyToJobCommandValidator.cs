using FluentValidation;

namespace JobApplication.Application.Features.JobCandidateApplications.Commands.ApplyToJob;

public class ApplyToJobCommandValidator : AbstractValidator<ApplyToJobCommand>
{
    public ApplyToJobCommandValidator()
    {
        RuleFor(x => x.JobId).GreaterThan(0);
        RuleFor(x => x.CvUrl).NotEmpty();
    }
}