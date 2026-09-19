using JobApplication.Application.DTOs.Auth;
using JobApplication.Domain.Common;

namespace JobApplication.Application.Services.Interfaces;

public interface IAuthService
{
    Task<Result<string>> RegisterAsync(RegisterDto dto);
    Task<Result<string>> RegisterRecruiterAsync(RegisterRecruiterDto dto);
    Task<Result<string>> LoginAsync(LoginDto dto);
}