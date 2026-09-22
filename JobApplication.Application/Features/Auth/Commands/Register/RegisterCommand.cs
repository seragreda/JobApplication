using JobApplication.Domain.Common;
using MediatR;

namespace JobApplication.Application.Features.Auth.Commands.Register;

public record RegisterCommand(string Email, string Password, string Name) : IRequest<Result<string>>;