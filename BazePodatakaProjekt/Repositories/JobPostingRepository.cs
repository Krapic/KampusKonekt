using BazePodatakaProjekt.Data;
using BazePodatakaProjekt.Models;
using Microsoft.EntityFrameworkCore;

namespace BazePodatakaProjekt.Repositories
{
    public class JobPostingRepository : IRepository<JobPosting>
    {
        private readonly ApplicationDbContext _context;
        public JobPostingRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(JobPosting entity)
        {
            await _context.JobPostings.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var jobPosting = await _context.JobPostings.FindAsync(id);
            if (jobPosting != null)
            {
                _context.JobPostings.Remove(jobPosting);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<JobPosting>> GetAllAsync()
        {
            return await _context.JobPostings
                .Include(jp => jp.Likes)
                .Include(jp => jp.Category)
                .Include(jp => jp.Images)
                .Include(jp => jp.Reviews)
                .ToListAsync();
        }

        public async Task<JobPosting> GetByIdAsync(int id)
        {
            var jobPosting = await _context.JobPostings.FindAsync(id);
            if (jobPosting == null)
            {
                throw new KeyNotFoundException();
            }
            return jobPosting;
        }

        public async Task UpdateAsync(JobPosting entity)
        {
            _context.JobPostings.Update(entity);
            await _context.SaveChangesAsync();
        }
    }
}
