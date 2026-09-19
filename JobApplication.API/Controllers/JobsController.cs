using JobApplication.Application.DTOs.Jobs;
using JobApplication.Application.Services.Interfaces;
using JobApplication.Domain.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JobApplication.API.Controllers;

[ApiController]
[Route("api/jobs")]
public class JobsController : ControllerBase
{
    private readonly IJobService _jobs;

    public JobsController(IJobService jobs) => _jobs = jobs;

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetAll()
        => Ok(await _jobs.GetAllOpenAsync());

    [HttpGet("{id:int}")]
    [AllowAnonymous]
    public async Task<IActionResult> Get(int id)
        => ToActionResult(await _jobs.GetByIdAsync(id));

    [HttpPost]
    [Authorize(Roles = "Recruiter")]
    public async Task<IActionResult> Create(CreateJobDto dto)
    {
        var result = await _jobs.CreateAsync(dto);
        if (!result.IsSuccess) return ToActionResult(result);
        return CreatedAtAction(nameof(Get), new { id = result.Value }, new { id = result.Value });
    }

    [HttpPut("{id:int}/close")]
    [Authorize(Roles = "Recruiter")]
    public async Task<IActionResult> Close(int id)
        => ToActionResult(await _jobs.CloseAsync(id));

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