using JobApplication.Application.Common.Interfaces;
using JobApplication.Application.DTOs.Applications;
using JobApplication.Application.Services.Interfaces;
using JobApplication.Domain.Common;
using JobApplication.Domain.Entities;
using JobApplication.Domain.Enums;

namespace JobApplication.Application.Services;

public class ApplicationService : IApplicationService
{
    private readonly IUnitOfWork _uow;
    private readonly ICurrentUserService _currentUser;

    public ApplicationService(IUnitOfWork uow, ICurrentUserService currentUser)
    {
        _uow = uow;
        _currentUser = currentUser;
    }

    public async Task<Result<int>> ApplyAsync(ApplyToJobDto dto)
    {
        var userId = _currentUser.UserId;
        if (string.IsNullOrEmpty(userId))
            return Result<int>.Fail("Unauthorized.", ErrorType.Unauthorized);

        var candidate = await _uow.Candidates.FirstOrDefaultAsync(c => c.UserId == userId);
        if (candidate is null)
            return Result<int>.Fail("Only candidates can apply.", ErrorType.Forbidden);

        var job = await _uow.Jobs.GetByIdAsync(dto.JobId);
        if (job is null) return Result<int>.Fail("Job not found.", ErrorType.NotFound);
        if (!job.IsActive) return Result<int>.Fail("Job is closed.", ErrorType.BadRequest);

        var existing = await _uow.Applications
            .FirstOrDefaultAsync(a => a.JobId == dto.JobId && a.CandidateId == candidate.Id);
        if (existing is not null)
            return Result<int>.Fail("Already applied.", ErrorType.Conflict);

        var application = new JobCandidateApplication
        {
            JobId = dto.JobId,
            CandidateId = candidate.Id,
            Status = JobApplicationStatus.Applied,
            AppliedAt = DateTime.UtcNow,
            StatusUpdatedAt = DateTime.UtcNow
        };

        candidate.CvUrl = dto.CvUrl;
        _uow.Candidates.Update(candidate);

        await _uow.Applications.AddAsync(application);
        await _uow.SaveChangesAsync();
        return Result<int>.Success(application.Id);
    }

    public async Task<Result> UpdateStatusAsync(int applicationId, UpdateApplicationStatusDto dto)
    {
        var userId = _currentUser.UserId;
        if (string.IsNullOrEmpty(userId))
            return Result.Fail("Unauthorized.", ErrorType.Unauthorized);

        var application = await _uow.Applications.GetByIdAsync(applicationId);
        if (application is null)
            return Result.Fail("Application not found.", ErrorType.NotFound);

        var job = await _uow.Jobs.GetByIdAsync(application.JobId);
        if (job is null) return Result.Fail("Job not found.", ErrorType.NotFound);

        var recruiter = await _uow.Recruiters.FirstOrDefaultAsync(r => r.UserId == userId);
        if (recruiter is null)
            return Result.Fail("Only recruiters can update status.", ErrorType.Forbidden);

        if (job.RecruiterId != recruiter.Id)
            return Result.Fail("You don't own this job.", ErrorType.Forbidden);

        if (dto.NewStatus == JobApplicationStatus.Cancelled)
            return Result.Fail("Use the cancel endpoint instead.", ErrorType.BadRequest);

        if (application.Status == JobApplicationStatus.Cancelled)
            return Result.Fail("Application already cancelled.", ErrorType.Conflict);

        // Forward-only transition
        if ((int)dto.NewStatus <= (int)application.Status)
            return Result.Fail("Only forward transitions are allowed.", ErrorType.BadRequest);

        application.Status = dto.NewStatus;
        application.StatusUpdatedAt = DateTime.UtcNow;

        _uow.Applications.Update(application);
        await _uow.SaveChangesAsync();
        return Result.Success();
    }

    public async Task<Result> CancelAsync(int applicationId)
    {
        var userId = _currentUser.UserId;
        if (string.IsNullOrEmpty(userId))
            return Result.Fail("Unauthorized.", ErrorType.Unauthorized);

        var application = await _uow.Applications.GetByIdAsync(applicationId);
        if (application is null)
            return Result.Fail("Application not found.", ErrorType.NotFound);

        var candidate = await _uow.Candidates.FirstOrDefaultAsync(c => c.UserId == userId);
        if (candidate is null)
            return Result.Fail("Only candidates can cancel.", ErrorType.Forbidden);

        if (application.CandidateId != candidate.Id)
            return Result.Fail("You don't own this application.", ErrorType.Forbidden);

        // Status must be Applied or UnderReview
        if (application.Status != JobApplicationStatus.Applied &&
            application.Status != JobApplicationStatus.UnderReview)
            return Result.Fail("Cannot cancel at this stage.", ErrorType.BadRequest);

        application.Status = JobApplicationStatus.Cancelled;
        application.StatusUpdatedAt = DateTime.UtcNow;
        application.CancelledAt = DateTime.UtcNow;

        _uow.Applications.Update(application);
        await _uow.SaveChangesAsync();
        return Result.Success();
    }
}