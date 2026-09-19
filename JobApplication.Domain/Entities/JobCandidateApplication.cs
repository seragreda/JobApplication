using JobApplication.Domain.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace JobApplication.Domain.Entities;

public class JobCandidateApplication
{
    public int Id { get; set; }

    public int CandidateId { get; set; }
    [ForeignKey(nameof(CandidateId))]
    public Candidate Candidate { get; set; } = null!;

    public int JobId { get; set; }
    [ForeignKey(nameof(JobId))]
    public Job Job { get; set; } = null!;

    public JobApplicationStatus Status { get; set; } = JobApplicationStatus.Applied;

    public DateTime AppliedAt { get; set; } = DateTime.UtcNow;
    public DateTime StatusUpdatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? CancelledAt { get; set; }
}