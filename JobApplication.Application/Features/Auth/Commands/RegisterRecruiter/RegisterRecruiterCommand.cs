using JobApplication.Domain.Common;
using MediatR;

namespace JobApplication.Application.Features.Auth.Commands.RegisterRecruiter;

public record RegisterRecruiterCommand(
    string Email,
    string Password,
    string Name,
    string CompanyName,
    string InvitationCode
) : IRequest<Result<string>>;