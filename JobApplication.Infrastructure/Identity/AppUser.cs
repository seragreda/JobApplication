using Microsoft.AspNetCore.Identity;

namespace JobApplication.Infrastructure.Identity;

public class AppUser : IdentityUser
{
    public string FullName { get; set; } = string.Empty;
}