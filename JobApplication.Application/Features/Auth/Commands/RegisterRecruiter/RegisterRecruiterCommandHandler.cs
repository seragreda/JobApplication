using JobApplication.Application.Common.Interfaces;
using JobApplication.Domain.Common;
using JobApplication.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Configuration;

namespace JobApplication.Application.Features.Auth.Commands.RegisterRecruiter;

public class RegisterRecruiterCommandHandler : IRequestHandler<RegisterRecruiterCommand, Result<string>>
{
    private readonly IIdentityService _identity;
    private readonly IUnitOfWork _uow;
    private readonly IConfiguration _config;

    public RegisterRecruiterCommandHandler(IIdentityService identity, IUnitOfWork uow, IConfiguration config)
    {
        _identity = identity;
        _uow = uow;
        _config = config;
    }

    public async Task<Result<string>> Handle(RegisterRecruiterCommand request, CancellationToken cancellationToken)
    {
        var expectedCode = _config["RecruiterInvitationCode"];
        if (string.IsNullOrEmpty(expectedCode) || request.InvitationCode != expectedCode)
            return Result<string>.Fail("Invalid invitation code.", ErrorType.Forbidden);

        var auth = await _identity.RegisterAsync(request.Email, request.Password, "Recruiter");
        if (!auth.Success || auth.UserId is null)
            return Result<string>.Fail(auth.Error ?? "Registration failed.", ErrorType.BadRequest);

        await _uow.Recruiters.AddAsync(new Recruiter
        {
            Name = request.Name,
            CompanyName = request.CompanyName,
            UserId = auth.UserId
        });
        await _uow.SaveChangesAsync();

        return Result<string>.Success(auth.Token!);
    }
}