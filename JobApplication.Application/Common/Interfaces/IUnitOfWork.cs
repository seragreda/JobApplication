using JobApplication.Domain.Entities;

namespace JobApplication.Application.Common.Interfaces;

// ‘Ì·‰« : IAsyncDisposable
public interface IUnitOfWork
{
    IGenericRepository<Job> Jobs { get; }
    IGenericRepository<Candidate> Candidates { get; }
    IGenericRepository<Recruiter> Recruiters { get; }
    IGenericRepository<JobCandidateApplication> Applications { get; }

    Task<int> SaveChangesAsync();
}