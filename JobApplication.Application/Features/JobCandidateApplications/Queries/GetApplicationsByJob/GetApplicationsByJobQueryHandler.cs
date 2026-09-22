using JobApplication.Application.Common.Interfaces;
using MediatR;

namespace JobApplication.Application.Features.JobCandidateApplications.Queries.GetApplicationsByJob;

public class GetApplicationsByJobQueryHandler
    : IRequestHandler<GetApplicationsByJobQuery, IReadOnlyList<ApplicationResponse>>
{
    private readonly IUnitOfWork _uow;

    public GetApplicationsByJobQueryHandler(IUnitOfWork uow) => _uow = uow;

    public async Task<IReadOnlyList<ApplicationResponse>> Handle(
        GetApplicationsByJobQuery request, CancellationToken cancellationToken)
    {
        var apps = await _uow.Applications.FindAsync(a => a.JobId == request.JobId);

        return apps.Select(a => new ApplicationResponse
        {
            Id = a.Id,
            JobId = a.JobId,
            CandidateId = a.CandidateId,
            Status = a.Status,
            AppliedAt = a.AppliedAt,
            StatusUpdatedAt = a.StatusUpdatedAt,
            CancelledAt = a.CancelledAt
        }).ToList();
    }
}