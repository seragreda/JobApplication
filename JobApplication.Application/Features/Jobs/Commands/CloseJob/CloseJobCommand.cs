using JobApplication.Domain.Common;
using MediatR;

namespace JobApplication.Application.Features.Jobs.Commands.CloseJob;

public record CloseJobCommand(int JobId) : IRequest<Result>;