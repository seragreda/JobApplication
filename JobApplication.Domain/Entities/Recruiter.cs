namespace JobApplication.Domain.Entities;

public class Recruiter
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string CompanyName { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;

    public ICollection<Job> Jobs { get; set; } = new List<Job>();
}