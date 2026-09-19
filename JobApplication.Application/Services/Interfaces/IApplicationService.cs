using JobApplication.Application.DTOs.Applications;
using JobApplication.Domain.Common;

namespace JobApplication.Application.Services.Interfaces;

public interface IApplicationService
{
    Task<Result<int>> ApplyAsync(ApplyToJobDto dto);
    Task<Result> UpdateStatusAsync(int applicationId, UpdateApplicationStatusDto dto);
    Task<Result> CancelAsync(int applicationId);
}