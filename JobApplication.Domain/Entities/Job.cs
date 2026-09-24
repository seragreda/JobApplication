namespace JobApplication.Domain.Entities;

public class Job
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;

    public int RecruiterId { get; set; }
    public Recruiter Recruiter { get; set; } = null!;

    public DateTime? ClosedAt { get; set; }
    public int? ClosedById { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<JobCandidateApplication> Applications { get; set; } = new List<JobCandidateApplication>();
}