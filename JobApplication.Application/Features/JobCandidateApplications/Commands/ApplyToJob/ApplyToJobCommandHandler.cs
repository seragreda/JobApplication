using JobApplication.Application.Common.Interfaces;
using JobApplication.Domain.Common;
using JobApplication.Domain.Entities;
using JobApplication.Domain.Enums;
using MediatR;

namespace JobApplication.Application.Features.JobCandidateApplications.Commands.ApplyToJob;

public class ApplyToJobCommandHandler : IRequestHandler<ApplyToJobCommand, Result<int>>
{
    private readonly IUnitOfWork _uow;
    private readonly ICurrentUserService _currentUser;

    public ApplyToJobCommandHandler(IUnitOfWork uow, ICurrentUserService currentUser)
    {
        _uow = uow;
        _currentUser = currentUser;
    }

    public async Task<Result<int>> Handle(ApplyToJobCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId;
        if (string.IsNullOrEmpty(userId))
            return Result<int>.Fail("Unauthorized.", ErrorType.Unauthorized);

        var candidate = await _uow.Candidates.FirstOrDefaultAsync(c => c.UserId == userId);
        if (candidate is null)
            return Result<int>.Fail("Only candidates can apply.", ErrorType.Forbidden);

        var job = await _uow.Jobs.GetByIdAsync(request.JobId);
        if (job is null)
            return Result<int>.Fail("Job not found.", ErrorType.NotFound);

        if (!job.IsActive)
            return Result<int>.Fail("Job is closed.", ErrorType.BadRequest);

        var existing = await _uow.Applications
            .FirstOrDefaultAsync(a => a.JobId == request.JobId && a.CandidateId == candidate.Id);

        if (existing is not null)
            return Result<int>.Fail("Already applied.", ErrorType.Conflict);

        var application = new JobCandidateApplication
        {
            JobId = request.JobId,
            CandidateId = candidate.Id,
            Status = JobApplicationStatus.Applied,
            AppliedAt = DateTime.UtcNow,
            StatusUpdatedAt = DateTime.UtcNow
        };

        candidate.CvUrl = request.CvUrl;
        _uow.Candidates.Update(candidate);

        await _uow.Applications.AddAsync(application);
        await _uow.SaveChangesAsync();

        return Result<int>.Success(application.Id);
    }
}