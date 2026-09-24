using JobApplication.Application.Common.Interfaces;
using JobApplication.Domain.Enums;
using Microsoft.Extensions.Logging;

namespace JobApplication.Infrastructure.BackgroundJobs;

public class DailyJobsReportJob
{
    private readonly IUnitOfWork _uow;
    private readonly ILogger<DailyJobsReportJob> _logger;

    public DailyJobsReportJob(
        IUnitOfWork uow,
        ILogger<DailyJobsReportJob> logger)
    {
        _uow = uow;
        _logger = logger;
    }

    public async Task ExecuteAsync()
    {
        _logger.LogInformation(" DailyJobsReportJob started at {Time}", DateTime.UtcNow);

        var yesterday = DateTime.UtcNow.AddDays(-1);

        var newJobs = await _uow.Jobs.FindAsync(j => j.CreatedAt >= yesterday);

        var newApplications = await _uow.Applications.FindAsync(a => a.AppliedAt >= yesterday);

        var cancelledApps = await _uow.Applications.FindAsync(a =>
            a.CancelledAt != null && a.CancelledAt >= yesterday);

        _logger.LogInformation(
            "DAILY REPORT ({Date}):\n" +
            " - New Jobs posted: {Jobs}\n" +
            " - New Applications: {Apps}\n" +
            " - Cancelled Applications: {Cancelled}",
            yesterday.ToString("yyyy-MM-dd"),
            newJobs.Count,
            newApplications.Count,
            cancelledApps.Count);

    }
}