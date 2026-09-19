using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using JobApplication.Application.Common.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace JobApplication.Infrastructure.Identity;

public class IdentityService : IIdentityService
{
    private readonly UserManager<AppUser> _userManager;
    private readonly IConfiguration _config;

    public IdentityService(UserManager<AppUser> userManager, IConfiguration config)
    {
        _userManager = userManager;
        _config = config;
    }

    public async Task<AuthResult> RegisterAsync(string email, string password, string role)
    {
        var existing = await _userManager.FindByEmailAsync(email);
        if (existing is not null)
            return new AuthResult(false, null, null, "Email already registered.");

        var user = new AppUser { UserName = email, Email = email, FullName = email };
        var result = await _userManager.CreateAsync(user, password);
        if (!result.Succeeded)
            return new AuthResult(false, null, null, string.Join("; ", result.Errors.Select(e => e.Description)));

        if (role != AppRoles.Recruiter && role != AppRoles.Candidate)
            role = AppRoles.Candidate;

        await _userManager.AddToRoleAsync(user, role);
        var token = GenerateToken(user.Id, user.Email!, role);
        return new AuthResult(true, token, user.Id, null);
    }

    public async Task<AuthResult> LoginAsync(string email, string password)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user is null) return new AuthResult(false, null, null, "Invalid credentials.");

        var ok = await _userManager.CheckPasswordAsync(user, password);
        if (!ok) return new AuthResult(false, null, null, "Invalid credentials.");

        var roles = await _userManager.GetRolesAsync(user);
        var role = roles.FirstOrDefault() ?? AppRoles.Candidate;
        var token = GenerateToken(user.Id, user.Email!, role);
        return new AuthResult(true, token, user.Id, null);
    }

    private string GenerateToken(string userId, string email, string role)
    {
        var keyString = _config["Jwt:Key"]
            ?? throw new InvalidOperationException("Jwt:Key is missing.");
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(keyString));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, userId),
            new Claim(ClaimTypes.NameIdentifier, userId),
            new Claim(JwtRegisteredClaimNames.Email, email),
            new Claim(ClaimTypes.Role, role)
        };

        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(6),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}