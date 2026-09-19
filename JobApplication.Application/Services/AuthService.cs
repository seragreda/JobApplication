using JobApplication.Application.Common.Interfaces;
using JobApplication.Application.DTOs.Auth;
using JobApplication.Application.Services.Interfaces;
using JobApplication.Domain.Common;
using JobApplication.Domain.Entities;
using Microsoft.Extensions.Configuration;

namespace JobApplication.Application.Services;

public class AuthService : IAuthService
{
    private readonly IIdentityService _identity;
    private readonly IUnitOfWork _uow;
    private readonly IConfiguration _config;

    public AuthService(IIdentityService identity, IUnitOfWork uow, IConfiguration config)
    {
        _identity = identity;
        _uow = uow;
        _config = config;
    }

    // «· ”ÃÌ· «·⁄«œÌ = Candidate œ«∆„«
    public async Task<Result<string>> RegisterAsync(RegisterDto dto)
    {
        var auth = await _identity.RegisterAsync(dto.Email, dto.Password, "Candidate");
        if (!auth.Success || auth.UserId is null)
            return Result<string>.Fail(auth.Error ?? "Registration failed.", ErrorType.BadRequest);

        await _uow.Candidates.AddAsync(new Candidate
        {
            Name = dto.Name,
            UserId = auth.UserId
        });
        await _uow.SaveChangesAsync();

        return Result<string>.Success(auth.Token!);
    }

    // ›ﬁÿ ·„‰ Ì„·ﬂ Invitation Code
    public async Task<Result<string>> RegisterRecruiterAsync(RegisterRecruiterDto dto)
    {
        var expectedCode = _config["RecruiterInvitationCode"];
        if (string.IsNullOrEmpty(expectedCode) || dto.InvitationCode != expectedCode)
            return Result<string>.Fail("Invalid invitation code.", ErrorType.Forbidden);

        var auth = await _identity.RegisterAsync(dto.Register.Email, dto.Register.Password, "Recruiter");
        if (!auth.Success || auth.UserId is null)
            return Result<string>.Fail(auth.Error ?? "Registration failed.", ErrorType.BadRequest);

        await _uow.Recruiters.AddAsync(new Recruiter
        {
            Name = dto.Register.Name,
            CompanyName = dto.CompanyName,
            UserId = auth.UserId
        });
        await _uow.SaveChangesAsync();

        return Result<string>.Success(auth.Token!);
    }

    public async Task<Result<string>> LoginAsync(LoginDto dto)
    {
        var auth = await _identity.LoginAsync(dto.Email, dto.Password);
        if (!auth.Success || auth.Token is null)
            return Result<string>.Fail(auth.Error ?? "Login failed.", ErrorType.BadRequest);

        return Result<string>.Success(auth.Token);
    }
}