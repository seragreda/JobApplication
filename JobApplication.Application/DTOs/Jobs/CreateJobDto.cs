using System.ComponentModel.DataAnnotations;

namespace JobApplication.Application.DTOs.Jobs;

public class CreateJobDto
{
    [Required, MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    [Required, MaxLength(4000)]
    public string Description { get; set; } = string.Empty;
}