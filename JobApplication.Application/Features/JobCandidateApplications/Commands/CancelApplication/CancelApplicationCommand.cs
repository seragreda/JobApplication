using JobApplication.Domain.Common;
using MediatR;

namespace JobApplication.Application.Features.JobCandidateApplications.Commands.CancelApplication;

public record CancelApplicationCommand(int ApplicationId) : IRequest<Result>;