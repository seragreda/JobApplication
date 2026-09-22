using JobApplication.Application.Common.Interfaces;
using JobApplication.Domain.Common;
using MediatR;

namespace JobApplication.Application.Features.Jobs.Commands.CloseJob;

public class CloseJobCommandHandler : IRequestHandler<CloseJobCommand, Result>
{
    private readonly IUnitOfWork _uow;
    private readonly ICurrentUserService _currentUser;

    public CloseJobCommandHandler(IUnitOfWork uow, ICurrentUserService currentUser)
    {
        _uow = uow;
        _currentUser = currentUser;
    }

    public async Task<Result> Handle(CloseJobCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId;
        if (string.IsNullOrEmpty(userId))
            return Result.Fail("Unauthorized.", ErrorType.Unauthorized);

        var job = await _uow.Jobs.GetByIdAsync(request.JobId);
        if (job is null)
            return Result.Fail("Job not found.", ErrorType.NotFound);

        var recruiter = await _uow.Recruiters.FirstOrDefaultAsync(r => r.UserId == userId);
        if (recruiter is null)
            return Result.Fail("Only recruiters can close jobs.", ErrorType.Forbidden);

        if (job.RecruiterId != recruiter.Id)
            return Result.Fail("You don't own this job.", ErrorType.Forbidden);

        if (!job.IsActive || job.ClosedAt is not null)
            return Result.Fail("Job already closed.", ErrorType.Conflict);

        job.IsActive = false;
        job.ClosedAt = DateTime.UtcNow;
        job.ClosedById = recruiter.Id;

        _uow.Jobs.Update(job);
        await _uow.SaveChangesAsync();

        return Result.Success();
    }
}