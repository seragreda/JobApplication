using JobApplication.Application.Common.Interfaces;
using JobApplication.Domain.Enums;
using Microsoft.Extensions.Logging;

namespace JobApplication.Infrastructure.BackgroundJobs;

public class CleanupCancelledApplicationsJob
{
    private readonly IUnitOfWork _uow;
    private readonly ILogger<CleanupCancelledApplicationsJob> _logger;

    public CleanupCancelledApplicationsJob(
        IUnitOfWork uow,
        ILogger<CleanupCancelledApplicationsJob> logger)
    {
        _uow = uow;
        _logger = logger;
    }

    public async Task ExecuteAsync()
    {
        _logger.LogInformation(" CleanupCancelledApplicationsJob started at {Time}", DateTime.UtcNow);

        var cutoff = DateTime.UtcNow.AddDays(-60);

        var oldCancelled = await _uow.Applications.FindAsync(a =>
            a.Status == JobApplicationStatus.Cancelled &&
            a.CancelledAt != null &&
            a.CancelledAt < cutoff);

        if (oldCancelled.Count == 0)
        {
            _logger.LogInformation("CleanupCancelledApplicationsJob: nothing to clean.");
            return;
        }

        foreach (var app in oldCancelled)
        {
            _uow.Applications.Remove(app);
        }

        await _uow.SaveChangesAsync();

        _logger.LogInformation(" CleanupCancelledApplicationsJob: deleted {Count} old cancelled applications.",
            oldCancelled.Count);
    }
}