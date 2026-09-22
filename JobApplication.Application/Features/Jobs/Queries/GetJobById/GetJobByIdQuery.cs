using JobApplication.Domain.Common;
using MediatR;

namespace JobApplication.Application.Features.Jobs.Queries.GetJobById;

public record GetJobByIdQuery(int JobId) : IRequest<Result<JobResponse>>;

public class JobResponse
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public DateTime? ClosedAt { get; set; }
}