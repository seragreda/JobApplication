using JobApplication.Application.DTOs.Auth;
using JobApplication.Application.Services.Interfaces;
using JobApplication.Domain.Common;
using Microsoft.AspNetCore.Mvc;

namespace JobApplication.API.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _auth;

    public AuthController(IAuthService auth) => _auth = auth;

    /// <summary>Register as a Candidate (default).</summary>
    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterDto dto)
        => ToActionResult(await _auth.RegisterAsync(dto));

    /// <summary>Register as a Recruiter — requires invitation code.</summary>
    [HttpPost("register-recruiter")]
    public async Task<IActionResult> RegisterRecruiter(RegisterRecruiterDto dto)
        => ToActionResult(await _auth.RegisterRecruiterAsync(dto));

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDto dto)
        => ToActionResult(await _auth.LoginAsync(dto));

    private IActionResult ToActionResult<T>(Result<T> result)
    {
        if (result.IsSuccess)
            return Ok(new { data = result.Value });

        return result.ErrorType switch
        {
            ErrorType.NotFound => NotFound(new { error = result.Error }),
            ErrorType.Unauthorized => Unauthorized(new { error = result.Error }),
            ErrorType.Forbidden => StatusCode(403, new { error = result.Error }),
            ErrorType.Conflict => Conflict(new { error = result.Error }),
            _ => BadRequest(new { error = result.Error })
        };
    }
}