using JobApplication.Application.DTOs.Applications;
using JobApplication.Application.Services.Interfaces;
using JobApplication.Domain.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JobApplication.API.Controllers;

[ApiController]
[Route("api/applications")]
[Authorize]
public class ApplicationsController : ControllerBase
{
    private readonly IApplicationService _apps;

    public ApplicationsController(IApplicationService apps) => _apps = apps;

    [HttpPost]
    [Authorize(Roles = "Candidate")]
    public async Task<IActionResult> Apply(ApplyToJobDto dto)
    {
        var result = await _apps.ApplyAsync(dto);
        if (!result.IsSuccess) return ToActionResult(result);
        return Ok(new { id = result.Value });
    }

    [HttpPut("{id:int}/status")]
    [Authorize(Roles = "Recruiter")]
    public async Task<IActionResult> UpdateStatus(int id, UpdateApplicationStatusDto dto)
        => ToActionResult(await _apps.UpdateStatusAsync(id, dto));

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Candidate")]
    public async Task<IActionResult> Cancel(int id)
        => ToActionResult(await _apps.CancelAsync(id));

    private IActionResult ToActionResult<T>(Result<T> result)
    {
        if (result.IsSuccess) return Ok(result.Value);
        return result.ErrorType switch
        {
            ErrorType.NotFound => NotFound(new { error = result.Error }),
            ErrorType.Unauthorized => Unauthorized(new { error = result.Error }),
            ErrorType.Forbidden => StatusCode(403, new { error = result.Error }),
            ErrorType.Conflict => Conflict(new { error = result.Error }),
            _ => BadRequest(new { error = result.Error })
        };
    }

    private IActionResult ToActionResult(Result result)
    {
        if (result.IsSuccess) return NoContent();
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