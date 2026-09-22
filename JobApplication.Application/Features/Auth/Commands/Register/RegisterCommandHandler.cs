using JobApplication.Application.Common.Interfaces;
using JobApplication.Domain.Common;
using JobApplication.Domain.Entities;
using MediatR;

namespace JobApplication.Application.Features.Auth.Commands.Register;

public class RegisterCommandHandler : IRequestHandler<RegisterCommand, Result<string>>
{
    private readonly IIdentityService _identity;
    private readonly IUnitOfWork _uow;

    public RegisterCommandHandler(IIdentityService identity, IUnitOfWork uow)
    {
        _identity = identity;
        _uow = uow;
    }

    public async Task<Result<string>> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        var auth = await _identity.RegisterAsync(request.Email, request.Password, "Candidate");
        if (!auth.Success || auth.UserId is null)
            return Result<string>.Fail(auth.Error ?? "Registration failed.", ErrorType.BadRequest);

        await _uow.Candidates.AddAsync(new Candidate
        {
            Name = request.Name,
            UserId = auth.UserId
        });
        await _uow.SaveChangesAsync();

        return Result<string>.Success(auth.Token!);
    }
}