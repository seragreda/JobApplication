using JobApplication.Domain.Common;
using JobApplication.Domain.Enums;
using MediatR;

namespace JobApplication.Application.Features.JobCandidateApplications.Commands.UpdateApplicationStatus;

public record UpdateApplicationStatusCommand(int ApplicationId, JobApplicationStatus NewStatus) : IRequest<Result>;