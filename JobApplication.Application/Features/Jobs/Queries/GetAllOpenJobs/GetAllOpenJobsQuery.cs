using JobApplication.Application.Features.Jobs.Queries.GetJobById;
using MediatR;

namespace JobApplication.Application.Features.Jobs.Queries.GetAllOpenJobs;

public record GetAllOpenJobsQuery : IRequest<IReadOnlyList<JobResponse>>;