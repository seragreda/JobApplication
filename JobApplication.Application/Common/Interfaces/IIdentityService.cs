namespace JobApplication.Application.Common.Interfaces;

public record AuthResult(bool Success, string? Token, string? UserId, string? Error);

public interface IIdentityService
{
    Task<AuthResult> RegisterAsync(string email, string password, string role);
    Task<AuthResult> LoginAsync(string email, string password);
}