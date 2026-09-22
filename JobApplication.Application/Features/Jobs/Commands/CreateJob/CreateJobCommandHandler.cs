using JobApplication.Application.Common.Interfaces;
using JobApplication.Domain.Common;
using JobApplication.Domain.Entities;
using MediatR;

namespace JobApplication.Application.Features.Jobs.Commands.CreateJob;

public class CreateJobCommandHandler : IRequestHandler<CreateJobCommand, Result<int>>
{
    private readonly IUnitOfWork _uow;
    private readonly ICurrentUserService _currentUser;

    public CreateJobCommandHandler(IUnitOfWork uow, ICurrentUserService currentUser)
    {
        _uow = uow;
        _currentUser = currentUser;
    }

    public async Task<Result<int>> Handle(CreateJobCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId;
        if (string.IsNullOrEmpty(userId))
            return Result<int>.Fail("Unauthorized.", ErrorType.Unauthorized);

        var recruiter = await _uow.Recruiters.FirstOrDefaultAsync(r => r.UserId == userId);
        if (recruiter is null)
            return Result<int>.Fail("Only recruiters can create jobs.", ErrorType.Forbidden);

        var job = new Job
        {
            Title = request.Title,
            Description = request.Description,
            IsActive = true,
            RecruiterId = recruiter.Id
        };

        await _uow.Jobs.AddAsync(job);
        await _uow.SaveChangesAsync();

        return Result<int>.Success(job.Id);
    }
}