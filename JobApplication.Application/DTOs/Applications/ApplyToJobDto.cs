using System.ComponentModel.DataAnnotations;

namespace JobApplication.Application.DTOs.Applications;

public class ApplyToJobDto
{
    [Required]
    public int JobId { get; set; }

    [Required, Url]
    public string CvUrl { get; set; } = string.Empty;
}