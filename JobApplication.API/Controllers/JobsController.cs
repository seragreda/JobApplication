using JobApplication.Application.DTOs.Jobs;
using JobApplication.Application.Features.Jobs.Commands.CloseJob;
using JobApplication.Application.Features.Jobs.Commands.CreateJob;
using JobApplication.Application.Features.Jobs.Queries.GetAllOpenJobs;
using JobApplication.Application.Features.Jobs.Queries.GetJobById;
using JobApplication.Domain.Common;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JobApplication.API.Controllers;

[ApiController]
[Route("api/jobs")]
public class JobsController : ControllerBase
{
    private readonly IMediator _mediator;

    public JobsController(IMediator mediator) => _mediator = mediator;

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetAll()
        => Ok(await _mediator.Send(new GetAllOpenJobsQuery()));

    [HttpGet("{id:int}")]
    [AllowAnonymous]
    public async Task<IActionResult> Get(int id)
        => ToActionResult(await _mediator.Send(new GetJobByIdQuery(id)));

    [HttpPost]
    [Authorize(Roles = "Recruiter")]
    public async Task<IActionResult> Create(CreateJobDto dto)
    {
        var result = await _mediator.Send(new CreateJobCommand(dto.Title, dto.Description));
        if (!result.IsSuccess) return ToActionResult(result);
        return CreatedAtAction(nameof(Get), new { id = result.Value }, new { id = result.Value });
    }

    [HttpPut("{id:int}/close")]
    [Authorize(Roles = "Recruiter")]
    public async Task<IActionResult> Close(int id)
        => ToActionResult(await _mediator.Send(new CloseJobCommand(id)));

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