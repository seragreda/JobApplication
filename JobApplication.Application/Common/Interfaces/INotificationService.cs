namespace JobApplication.Application.Common.Interfaces;

public interface INotificationService
{
    Task NotifyCandidateAsync(int applicationId);
}