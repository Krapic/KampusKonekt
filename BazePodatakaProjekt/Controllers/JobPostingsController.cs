using BazePodatakaProjekt.Constants;
using BazePodatakaProjekt.Data;
using BazePodatakaProjekt.Models;
using BazePodatakaProjekt.Repositories;
using BazePodatakaProjekt.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BazePodatakaProjekt.Controllers
{
    [Authorize]
    public class JobPostingsController : Controller
    {
        private readonly IRepository<JobPosting> _repository;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ApplicationDbContext _context;

        public JobPostingsController(IRepository<JobPosting> repository, UserManager<IdentityUser> userManager, ApplicationDbContext context)
        {
            _repository = repository;
            _userManager = userManager;
            _context = context;
        }

        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            var currentUserId = _userManager.GetUserId(User);
            var jobPostings = await _context.JobPostings
                .Include(jp => jp.Likes)
                .Include(jp => jp.Category)
                .Include(jp => jp.Images)
                .Include(jp => jp.Reviews)
                .Include(jp => jp.User) // Dodajte ovu liniju za uključivanje korisničkih podataka
                .ToListAsync();

            var viewModel = jobPostings.Select(jp => new JobPostingWithFollowStatusViewModel
            {
                JobPosting = jp,
                IsFollowing = _context.UserFollows.Any(uf => uf.FollowerId == currentUserId && uf.FollowedId == jp.UserId)
            }).ToList();

            return View(viewModel);
        }

        [Authorize(Roles = "Admin, Employer")]
        public IActionResult Create()
        {
            ViewBag.Categories = _context.Categories.ToList(); // Osiguraj da su kategorije dostupne u View-u
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> Create(JobPostingViewModel jobPostingVm)
        {
            if (ModelState.IsValid)
            {
                var jobPosting = new JobPosting
                {
                    Title = jobPostingVm.Title,
                    Description = jobPostingVm.Description,
                    Company = jobPostingVm.Company,
                    Location = jobPostingVm.Location,
                    CategoryId = jobPostingVm.CategoryId,
                    Price = jobPostingVm.Price,
                    Condition = jobPostingVm.Condition,
                    UserId = _userManager.GetUserId(User)
                };

                _context.JobPostings.Add(jobPosting);
                await _context.SaveChangesAsync();

                // Spremanje slike ako postoji
                if (jobPostingVm.Image != null && jobPostingVm.Image.Length > 0)
                {
                    var imagePath = "/uploads/" + Guid.NewGuid() + Path.GetExtension(jobPostingVm.Image.FileName);
                    using (var stream = new FileStream("wwwroot" + imagePath, FileMode.Create))
                    {
                        await jobPostingVm.Image.CopyToAsync(stream);
                    }

                    var image = new Image
                    {
                        JobPostingId = jobPosting.Id,
                        Url = imagePath
                    };
                    _context.Images.Add(image);
                    await _context.SaveChangesAsync();
                }

                return RedirectToAction(nameof(Index));
            }
            ViewBag.Categories = _context.Categories.ToList();
            return View(jobPostingVm);
        }

        [HttpDelete]
        [Authorize(Roles = "Admin, Employer")]
        public async Task<IActionResult> Delete(int id)
        {
            var jobPosting = await _context.JobPostings.FindAsync(id);
            if (jobPosting == null)
            {
                return NotFound();
            }

            // Prvo obriši sve lajkove povezane s ovim oglasom
            var likes = _context.Likes.Where(l => l.JobPostingId == id);
            _context.Likes.RemoveRange(likes);
            await _context.SaveChangesAsync();

            // Sada obriši sam oglas
            _context.JobPostings.Remove(jobPosting);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Oglas uspješno obrisan." });
        }


        [HttpPost]
        [Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> Approve(int id)
        {
            var jobPosting = await _repository.GetByIdAsync(id);
            if (jobPosting == null)
            {
                return NotFound();
            }

            jobPosting.IsApproved = true;
            await _repository.UpdateAsync(jobPosting);

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Like(int jobPostingId)
        {
            var userId = _userManager.GetUserId(User);
            Console.WriteLine($"User {userId} is trying to like job posting {jobPostingId}");

            var existingLike = await _context.Likes
                .FirstOrDefaultAsync(l => l.JobPostingId == jobPostingId && l.UserId == userId);

            if (existingLike == null)
            {
                Console.WriteLine("Adding new like...");
                var like = new Like
                {
                    UserId = userId,
                    JobPostingId = jobPostingId
                };
                _context.Likes.Add(like);
            }
            else
            {
                Console.WriteLine("Removing existing like...");
                _context.Likes.Remove(existingLike);
            }

            await _context.SaveChangesAsync();

            // Ažuriraj broj lajkova u JobPosting
            var jobPosting = await _context.JobPostings
                .Include(jp => jp.Likes)
                .FirstOrDefaultAsync(jp => jp.Id == jobPostingId);

            if (jobPosting != null)
            {
                jobPosting.LikeCount = jobPosting.Likes.Count;
                await _context.SaveChangesAsync();
                Console.WriteLine($"New like count for job posting {jobPostingId}: {jobPosting.LikeCount}");
            }
            else
            {
                Console.WriteLine($"Job posting {jobPostingId} not found.");
            }

            return Json(new { LikeCount = jobPosting?.LikeCount ?? 0 });
        }
    }
}
