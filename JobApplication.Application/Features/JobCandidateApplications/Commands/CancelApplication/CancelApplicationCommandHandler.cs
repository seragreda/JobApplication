using Hangfire;
using JobApplication.Application.Common.Interfaces;
using JobApplication.Domain.Common;
using JobApplication.Domain.Enums;
using MediatR;

namespace JobApplication.Application.Features.JobCandidateApplications.Commands.CancelApplication;

public class CancelApplicationCommandHandler : IRequestHandler<CancelApplicationCommand, Result>
{
    private readonly IUnitOfWork _uow;
    private readonly ICurrentUserService _currentUser;

    public CancelApplicationCommandHandler(IUnitOfWork uow, ICurrentUserService currentUser)
    {
        _uow = uow;
        _currentUser = currentUser;
    }

    public async Task<Result> Handle(CancelApplicationCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId;
        if (string.IsNullOrEmpty(userId))
            return Result.Fail("Unauthorized.", ErrorType.Unauthorized);

        var application = await _uow.Applications.GetByIdAsync(request.ApplicationId);
        if (application is null)
            return Result.Fail("Application not found.", ErrorType.NotFound);

        var candidate = await _uow.Candidates.FirstOrDefaultAsync(c => c.UserId == userId);
        if (candidate is null)
            return Result.Fail("Only candidates can cancel.", ErrorType.Forbidden);

        if (application.CandidateId != candidate.Id)
            return Result.Fail("You don't own this application.", ErrorType.Forbidden);

        if (application.Status != JobApplicationStatus.Applied &&
            application.Status != JobApplicationStatus.UnderReview)
            return Result.Fail("Cannot cancel at this stage.", ErrorType.BadRequest);

        application.Status = JobApplicationStatus.Cancelled;
        application.StatusUpdatedAt = DateTime.UtcNow;
        application.CancelledAt = DateTime.UtcNow;

        _uow.Applications.Update(application);
        await _uow.SaveChangesAsync();

        var applicationId = application.Id;
        BackgroundJob.Enqueue<INotificationService>(x => x.NotifyCandidateAsync(applicationId));

        return Result.Success();
    }
}