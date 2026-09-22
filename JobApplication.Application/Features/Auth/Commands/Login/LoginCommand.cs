using JobApplication.Domain.Common;
using MediatR;

namespace JobApplication.Application.Features.Auth.Commands.Login;

public record LoginCommand(string Email, string Password) : IRequest<Result<string>>;