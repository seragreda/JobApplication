using JobApplication.Application.DTOs.Jobs;
using JobApplication.Domain.Common;

namespace JobApplication.Application.Services.Interfaces;

public interface IJobService
{
    Task<Result<int>> CreateAsync(CreateJobDto dto);
    Task<Result> CloseAsync(int jobId);
    Task<Result<JobDto>> GetByIdAsync(int jobId);
    Task<IReadOnlyList<JobDto>> GetAllOpenAsync();
}