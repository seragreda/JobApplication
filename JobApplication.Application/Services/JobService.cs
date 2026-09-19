using JobApplication.Application.Common.Interfaces;
using JobApplication.Application.DTOs.Jobs;
using JobApplication.Application.Services.Interfaces;
using JobApplication.Domain.Common;
using JobApplication.Domain.Entities;

namespace JobApplication.Application.Services;

public class JobService : IJobService
{
    private readonly IUnitOfWork _uow;
    private readonly ICurrentUserService _currentUser;

    public JobService(IUnitOfWork uow, ICurrentUserService currentUser)
    {
        _uow = uow;
        _currentUser = currentUser;
    }

    public async Task<Result<int>> CreateAsync(CreateJobDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Title))
            return Result<int>.Fail("Title is required.", ErrorType.BadRequest);

        var userId = _currentUser.UserId;
        if (string.IsNullOrEmpty(userId))
            return Result<int>.Fail("Unauthorized.", ErrorType.Unauthorized);

        var recruiter = await _uow.Recruiters.FirstOrDefaultAsync(r => r.UserId == userId);
        if (recruiter is null)
            return Result<int>.Fail("Only recruiters can create jobs.", ErrorType.Forbidden);

        var job = new Job
        {
            Title = dto.Title,
            Description = dto.Description,
            IsActive = true,
            RecruiterId = recruiter.Id
        };

        await _uow.Jobs.AddAsync(job);
        await _uow.SaveChangesAsync();
        return Result<int>.Success(job.Id);
    }

    public async Task<Result> CloseAsync(int jobId)
    {
        var userId = _currentUser.UserId;
        if (string.IsNullOrEmpty(userId))
            return Result.Fail("Unauthorized.", ErrorType.Unauthorized);

        var job = await _uow.Jobs.GetByIdAsync(jobId);
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

    public async Task<Result<JobDto>> GetByIdAsync(int jobId)
    {
        var job = await _uow.Jobs.GetByIdAsync(jobId);
        if (job is null) return Result<JobDto>.Fail("Job not found.", ErrorType.NotFound);

        return Result<JobDto>.Success(new JobDto
        {
            Id = job.Id,
            Title = job.Title,
            Description = job.Description,
            IsActive = job.IsActive,
            ClosedAt = job.ClosedAt
        });
    }

    public async Task<IReadOnlyList<JobDto>> GetAllOpenAsync()
    {
        var jobs = await _uow.Jobs.FindAsync(j => j.IsActive);
        return jobs.Select(j => new JobDto
        {
            Id = j.Id,
            Title = j.Title,
            Description = j.Description,
            IsActive = j.IsActive,
            ClosedAt = j.ClosedAt
        }).ToList();
    }
}