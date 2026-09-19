using System.ComponentModel.DataAnnotations;
using JobApplication.Domain.Enums;

namespace JobApplication.Application.DTOs.Applications;

public class UpdateApplicationStatusDto
{
    [Required]
    [EnumDataType(typeof(JobApplicationStatus))]
    public JobApplicationStatus NewStatus { get; set; }
}