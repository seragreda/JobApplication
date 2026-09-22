using JobApplication.Application.DTOs.Auth;
using JobApplication.Application.Features.Auth.Commands.Login;
using JobApplication.Application.Features.Auth.Commands.Register;
using JobApplication.Application.Features.Auth.Commands.RegisterRecruiter;
using JobApplication.Domain.Common;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace JobApplication.API.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;

    public AuthController(IMediator mediator) => _mediator = mediator;

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterDto dto)
    {
        var result = await _mediator.Send(new RegisterCommand(dto.Email, dto.Password, dto.Name));
        return ToActionResult(result);
    }

    [HttpPost("register-recruiter")]
    public async Task<IActionResult> RegisterRecruiter(RegisterRecruiterDto dto)
    {
        var result = await _mediator.Send(new RegisterRecruiterCommand(
            dto.Register.Email, dto.Register.Password, dto.Register.Name,
            dto.CompanyName, dto.InvitationCode));
        return ToActionResult(result);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDto dto)
    {
        var result = await _mediator.Send(new LoginCommand(dto.Email, dto.Password));
        return ToActionResult(result);
    }

    private IActionResult ToActionResult<T>(Result<T> result)
    {
        if (result.IsSuccess) return Ok(new { data = result.Value });
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