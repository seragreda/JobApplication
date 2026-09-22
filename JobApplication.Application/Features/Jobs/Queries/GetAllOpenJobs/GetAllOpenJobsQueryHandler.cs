using JobApplication.Application.Common.Interfaces;
using JobApplication.Application.Features.Jobs.Queries.GetJobById;
using MediatR;

namespace JobApplication.Application.Features.Jobs.Queries.GetAllOpenJobs;

public class GetAllOpenJobsQueryHandler : IRequestHandler<GetAllOpenJobsQuery, IReadOnlyList<JobResponse>>
{
    private readonly IUnitOfWork _uow;

    public GetAllOpenJobsQueryHandler(IUnitOfWork uow) => _uow = uow;

    public async Task<IReadOnlyList<JobResponse>> Handle(GetAllOpenJobsQuery request, CancellationToken cancellationToken)
    {
        var jobs = await _uow.Jobs.FindAsync(j => j.IsActive);

        return jobs.Select(j => new JobResponse
        {
            Id = j.Id,
            Title = j.Title,
            Description = j.Description,
            IsActive = j.IsActive,
            ClosedAt = j.ClosedAt
        }).ToList();
    }
}