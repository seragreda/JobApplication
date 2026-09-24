using JobApplication.Application.Common.Interfaces;
using Microsoft.Extensions.Logging;

namespace JobApplication.Infrastructure.Notifications;

public class EmailNotificationService : INotificationService
{
    private readonly ILogger<EmailNotificationService> _logger;

    public EmailNotificationService(ILogger<EmailNotificationService> logger)
    {
        _logger = logger;
    }

    public async Task NotifyCandidateAsync(int applicationId)
    {
        
        _logger.LogInformation(" [Hangfire] Email sent to candidate for application {ApplicationId} at {Time}",
            applicationId, DateTime.UtcNow);

        await Task.Delay(500); 
    }
}