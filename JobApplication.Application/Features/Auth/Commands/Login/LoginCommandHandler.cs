using JobApplication.Application.Common.Interfaces;
using JobApplication.Domain.Common;
using MediatR;

namespace JobApplication.Application.Features.Auth.Commands.Login;

public class LoginCommandHandler : IRequestHandler<LoginCommand, Result<string>>
{
    private readonly IIdentityService _identity;

    public LoginCommandHandler(IIdentityService identity) => _identity = identity;

    public async Task<Result<string>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var auth = await _identity.LoginAsync(request.Email, request.Password);
        if (!auth.Success || auth.Token is null)
            return Result<string>.Fail(auth.Error ?? "Login failed.", ErrorType.BadRequest);

        return Result<string>.Success(auth.Token);
    }
}