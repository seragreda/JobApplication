using JobApplication.Application.DTOs.Applications;
using JobApplication.Application.Features.JobCandidateApplications.Commands.ApplyToJob;
using JobApplication.Application.Features.JobCandidateApplications.Commands.CancelApplication;
using JobApplication.Application.Features.JobCandidateApplications.Commands.UpdateApplicationStatus;
using JobApplication.Application.Features.JobCandidateApplications.Queries.GetApplicationsByJob;
using JobApplication.Domain.Common;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JobApplication.API.Controllers;

[ApiController]
[Route("api/applications")]
[Authorize]
public class ApplicationsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ApplicationsController(IMediator mediator) => _mediator = mediator;

    [HttpPost]
    [Authorize(Roles = "Candidate")]
    public async Task<IActionResult> Apply(ApplyToJobDto dto)
    {
        var result = await _mediator.Send(new ApplyToJobCommand(dto.JobId, dto.CvUrl));
        if (!result.IsSuccess) return ToActionResult(result);
        return Ok(new { id = result.Value });
    }

    [HttpPut("{id:int}/status")]
    [Authorize(Roles = "Recruiter")]
    public async Task<IActionResult> UpdateStatus(int id, UpdateApplicationStatusDto dto)
        => ToActionResult(await _mediator.Send(new UpdateApplicationStatusCommand(id, dto.NewStatus)));

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Candidate")]
    public async Task<IActionResult> Cancel(int id)
        => ToActionResult(await _mediator.Send(new CancelApplicationCommand(id)));

    [HttpGet("job/{jobId:int}")]
    [Authorize(Roles = "Recruiter")]
    public async Task<IActionResult> GetByJob(int jobId)
        => Ok(await _mediator.Send(new GetApplicationsByJobQuery(jobId)));

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