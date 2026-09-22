using JobApplication.Application.Common.Interfaces;
using JobApplication.Domain.Common;
using MediatR;

namespace JobApplication.Application.Features.Jobs.Queries.GetJobById;

public class GetJobByIdQueryHandler : IRequestHandler<GetJobByIdQuery, Result<JobResponse>>
{
    private readonly IUnitOfWork _uow;

    public GetJobByIdQueryHandler(IUnitOfWork uow) => _uow = uow;

    public async Task<Result<JobResponse>> Handle(GetJobByIdQuery request, CancellationToken cancellationToken)
    {
        var job = await _uow.Jobs.GetByIdAsync(request.JobId);
        if (job is null)
            return Result<JobResponse>.Fail("Job not found.", ErrorType.NotFound);

        return Result<JobResponse>.Success(new JobResponse
        {
            Id = job.Id,
            Title = job.Title,
            Description = job.Description,
            IsActive = job.IsActive,
            ClosedAt = job.ClosedAt
        });
    }
}