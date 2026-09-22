using JobApplication.Application.Common.Interfaces;
using JobApplication.Domain.Common;
using JobApplication.Domain.Enums;
using MediatR;

namespace JobApplication.Application.Features.JobCandidateApplications.Commands.UpdateApplicationStatus;

public class UpdateApplicationStatusCommandHandler
    : IRequestHandler<UpdateApplicationStatusCommand, Result>
{
    private readonly IUnitOfWork _uow;
    private readonly ICurrentUserService _currentUser;

    public UpdateApplicationStatusCommandHandler(IUnitOfWork uow, ICurrentUserService currentUser)
    {
        _uow = uow;
        _currentUser = currentUser;
    }

    public async Task<Result> Handle(UpdateApplicationStatusCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId;
        if (string.IsNullOrEmpty(userId))
            return Result.Fail("Unauthorized.", ErrorType.Unauthorized);

        var application = await _uow.Applications.GetByIdAsync(request.ApplicationId);
        if (application is null)
            return Result.Fail("Application not found.", ErrorType.NotFound);

        var job = await _uow.Jobs.GetByIdAsync(application.JobId);
        if (job is null)
            return Result.Fail("Job not found.", ErrorType.NotFound);

        var recruiter = await _uow.Recruiters.FirstOrDefaultAsync(r => r.UserId == userId);
        if (recruiter is null)
            return Result.Fail("Only recruiters can update status.", ErrorType.Forbidden);

        if (job.RecruiterId != recruiter.Id)
            return Result.Fail("You don't own this job.", ErrorType.Forbidden);

        if (request.NewStatus == JobApplicationStatus.Cancelled)
            return Result.Fail("Use the cancel endpoint instead.", ErrorType.BadRequest);

        if (application.Status == JobApplicationStatus.Cancelled)
            return Result.Fail("Application already cancelled.", ErrorType.Conflict);

        if ((int)request.NewStatus <= (int)application.Status)
            return Result.Fail("Only forward transitions are allowed.", ErrorType.BadRequest);

        application.Status = request.NewStatus;
        application.StatusUpdatedAt = DateTime.UtcNow;

        _uow.Applications.Update(application);
        await _uow.SaveChangesAsync();

        return Result.Success();
    }
}