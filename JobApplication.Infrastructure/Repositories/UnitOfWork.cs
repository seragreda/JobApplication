using JobApplication.Application.Common.Interfaces;
using JobApplication.Domain.Entities;
using JobApplication.Infrastructure.Persistence;

namespace JobApplication.Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;

    public IGenericRepository<Job> Jobs { get; }
    public IGenericRepository<Candidate> Candidates { get; }
    public IGenericRepository<Recruiter> Recruiters { get; }
    public IGenericRepository<JobCandidateApplication> Applications { get; }

    public UnitOfWork(ApplicationDbContext context)
    {
        _context = context;
        Jobs = new GenericRepository<Job>(context);
        Candidates = new GenericRepository<Candidate>(context);
        Recruiters = new GenericRepository<Recruiter>(context);
        Applications = new GenericRepository<JobCandidateApplication>(context);
    }

    public Task<int> SaveChangesAsync() => _context.SaveChangesAsync();

    // ‘Ì·‰« DisposeAsync
}