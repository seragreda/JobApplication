using JobApplication.Domain.Enums;
using MediatR;

namespace JobApplication.Application.Features.JobCandidateApplications.Queries.GetApplicationsByJob;

public record GetApplicationsByJobQuery(int JobId) : IRequest<IReadOnlyList<ApplicationResponse>>;

public class ApplicationResponse
{
    public int Id { get; set; }
    public int JobId { get; set; }
    public string JobTitle { get; set; } = string.Empty;
    public int CandidateId { get; set; }
    public string CandidateName { get; set; } = string.Empty;
    public JobApplicationStatus Status { get; set; }
    public DateTime AppliedAt { get; set; }
    public DateTime StatusUpdatedAt { get; set; }
    public DateTime? CancelledAt { get; set; }
}