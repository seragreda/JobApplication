using System.ComponentModel.DataAnnotations;

namespace JobApplication.Application.DTOs.Auth;

public class RegisterRecruiterDto
{
    [Required]
    public RegisterDto Register { get; set; } = new();

    [Required]
    public string InvitationCode { get; set; } = string.Empty;

    [Required, MaxLength(200)]
    public string CompanyName { get; set; } = string.Empty;
}