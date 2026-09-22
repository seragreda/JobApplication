using JobApplication.Domain.Common;
using MediatR;

namespace JobApplication.Application.Features.JobCandidateApplications.Commands.ApplyToJob;

public record ApplyToJobCommand(int JobId, string CvUrl) : IRequest<Result<int>>;