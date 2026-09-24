using JobApplication.Application.Common.Interfaces;
using Microsoft.Extensions.Logging;

namespace JobApplication.Infrastructure.BackgroundJobs;

public class CloseStaleJobsJob
{
    private readonly IUnitOfWork _uow;
    private readonly ILogger<CloseStaleJobsJob> _logger;

    public CloseStaleJobsJob(IUnitOfWork uow, ILogger<CloseStaleJobsJob> logger)
    {
        _uow = uow;
        _logger = logger;
    }

    public async Task ExecuteAsync()
    {
        _logger.LogInformation(" CloseStaleJobsJob started at {Time}", DateTime.UtcNow);

        var cutoff = DateTime.UtcNow.AddDays(-30);

        var staleJobs = await _uow.Jobs.FindAsync(j =>
            j.IsActive && j.CreatedAt < cutoff);

        if (staleJobs.Count == 0)
        {
            _logger.LogInformation("CloseStaleJobsJob: no stale jobs found.");
            return;
        }

        foreach (var job in staleJobs)
        {
            job.IsActive = false;
            job.ClosedAt = DateTime.UtcNow;
            job.ClosedById = null; 
            _uow.Jobs.Update(job);
        }

        await _uow.SaveChangesAsync();

        _logger.LogInformation(" CloseStaleJobsJob: closed {Count} stale jobs.", staleJobs.Count);
    }
}