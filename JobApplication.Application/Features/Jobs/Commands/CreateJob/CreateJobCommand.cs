using JobApplication.Domain.Common;
using MediatR;

namespace JobApplication.Application.Features.Jobs.Commands.CreateJob;

public record CreateJobCommand(string Title, string Description) : IRequest<Result<int>>;