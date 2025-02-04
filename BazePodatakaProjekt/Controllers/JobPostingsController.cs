using System.Security.Claims;
using BazePodatakaProjekt.Constants;
using BazePodatakaProjekt.Data;
using BazePodatakaProjekt.Models;
using BazePodatakaProjekt.Repositories;
using BazePodatakaProjekt.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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
        public async Task<IActionResult> Index(string sortOrder)
        {
            ViewData["CategorySortParam"] = sortOrder == "category" ? "category_desc" : "category";
            ViewData["PriceSortParam"] = sortOrder == "price" ? "price_desc" : "price";
            ViewData["DateSortParam"] = sortOrder == "date" ? "date_desc" : "date";

            var currentUserId = _userManager.GetUserId(User);

            var jobPostings = _context.JobPostings
                .Include(jp => jp.Likes)
                .Include(jp => jp.Category)
                .Include(jp => jp.Images)
                .Include(jp => jp.Reviews)
                .Include(jp => jp.User)
                .AsQueryable();

            switch (sortOrder)
            {
                case "category":
                    jobPostings = jobPostings.OrderBy(jp => jp.Category.Name);
                    break;
                case "category_desc":
                    jobPostings = jobPostings.OrderByDescending(jp => jp.Category.Name);
                    break;
                case "price":
                    jobPostings = jobPostings.OrderBy(jp => jp.Price);
                    break;
                case "price_desc":
                    jobPostings = jobPostings.OrderByDescending(jp => jp.Price);
                    break;
                case "date":
                    jobPostings = jobPostings.OrderBy(jp => jp.PostedDate);
                    break;
                case "date_desc":
                    jobPostings = jobPostings.OrderByDescending(jp => jp.PostedDate);
                    break;
                default:
                    jobPostings = jobPostings.OrderByDescending(jp => jp.PostedDate);
                    break;
            }

            var viewModel = await jobPostings
                .Select(jp => new JobPostingWithFollowStatusViewModel
                {
                    JobPosting = jp,
                    IsFollowing = _context.UserFollows.Any(uf => uf.FollowerId == currentUserId && uf.FollowedId == jp.UserId), Reviews = jp.Reviews
                }).ToListAsync();

            return View(viewModel);
        }


        [Authorize(Roles = "Admin, Employer")]
        public IActionResult Create()
        {
            ViewBag.Categories = _context.Categories.ToList();
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

            var likes = _context.Likes.Where(l => l.JobPostingId == id);
            _context.Likes.RemoveRange(likes);
            await _context.SaveChangesAsync();

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

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var jobPosting = await _context.JobPostings.FindAsync(id);
            if (jobPosting == null || jobPosting.UserId != User.FindFirstValue(ClaimTypes.NameIdentifier))
            {
                return NotFound();
            }

            var viewModel = new JobPostingViewModel
            {
                Id = jobPosting.Id,
                Title = jobPosting.Title,
                Description = jobPosting.Description,
                Price = jobPosting.Price,
                CategoryId = jobPosting.CategoryId,
                Condition = jobPosting.Condition,
                Company = jobPosting.Company,
                Location = jobPosting.Location
            };

            ViewData["Categories"] = new SelectList(_context.Categories, "Id", "Name", jobPosting.CategoryId);

            return View("~/Views/JobPostings/Edit.cshtml", viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(JobPostingViewModel model)
        {
            Console.WriteLine($"Edit metoda pozvana za ID: {model.Id}");

            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                foreach (var error in errors)
                {
                    Console.WriteLine($"ModelState Error: {error}");
                }

                ViewData["Categories"] = new SelectList(_context.Categories, "Id", "Name", model.CategoryId);
                return View("~/Views/JobPostings/Edit.cshtml", model);
            }

            var jobPosting = await _context.JobPostings.FindAsync(model.Id);
            if (jobPosting == null || jobPosting.UserId != User.FindFirstValue(ClaimTypes.NameIdentifier))
            {
                Console.WriteLine("Greška: Oglas nije pronađen ili korisnik nema dozvolu.");
                return NotFound();
            }

            jobPosting.Title = model.Title;
            jobPosting.Description = model.Description;
            jobPosting.Price = model.Price;
            jobPosting.CategoryId = model.CategoryId;
            jobPosting.Condition = model.Condition;
            jobPosting.Company = model.Company;
            jobPosting.Location = model.Location;

            await _context.SaveChangesAsync();

            Console.WriteLine("Podaci su uspješno ažurirani!");

            return RedirectToAction(nameof(Index));
        }

        [Authorize]
        public async Task<IActionResult> LikedPosts()
        {
            var currentUserId = _userManager.GetUserId(User);

            var likedPosts = await _context.Likes
                .Where(like => like.UserId == currentUserId)
                .Include(like => like.JobPosting)
                .ThenInclude(jp => jp.Category)
                .Select(like => like.JobPosting)
                .ToListAsync();


            return View(likedPosts);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddReview(int jobPostingId, int rating, string comment)
        {
            var currentUserId = _userManager.GetUserId(User);
            var currentUser = await _userManager.FindByIdAsync(currentUserId);

            var existingReview = await _context.Reviews
                .FirstOrDefaultAsync(r => r.JobPostingId == jobPostingId && r.UserId == currentUserId);

            if (existingReview != null)
            {
                return Json(new { success = false, message = "Već ste ostavili recenziju za ovaj oglas." });
            }

            var review = new Review
            {
                JobPostingId = jobPostingId,
                UserId = currentUserId,
                Rating = rating,
                Comment = comment,
                ReviewDate = DateTime.UtcNow
            };

            _context.Reviews.Add(review);
            await _context.SaveChangesAsync();

            return Json(new
            {
                success = true,
                message = "Recenzija uspješno dodana.",
                review = new
                {
                    UserName = currentUser.UserName,
                    Rating = review.Rating,
                    Comment = review.Comment,
                    ReviewDate = review.ReviewDate.ToShortDateString()
                }
            });
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetReviews(int jobPostingId)
        {
            var reviews = await _context.Reviews
                .Include(r => r.User)
                .Where(r => r.JobPostingId == jobPostingId)
                .OrderByDescending(r => r.ReviewDate)
                .Select(r => new
                {
                    id = r.Id,
                    userName = r.User.UserName,
                    rating = r.Rating,
                    comment = r.Comment,
                    reviewDate = r.ReviewDate.ToShortDateString(),
                    userId = r.UserId
                })
                .ToListAsync();

            return Json(reviews);
        }

        [HttpDelete]
        [Route("JobPostings/DeleteReview/{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteReview(int id)
        {
            Console.WriteLine($"Pokušaj brisanja recenzije s ID: {id}");

            var review = await _context.Reviews.FindAsync(id);
            if (review == null)
            {
                Console.WriteLine("Greška: Recenzija nije pronađena.");
                return NotFound(new { message = "Recenzija nije pronađena." });
            }

            _context.Reviews.Remove(review);
            await _context.SaveChangesAsync();

            Console.WriteLine("Recenzija uspješno obrisana.");
            return Ok(new { message = "Recenzija uspješno obrisana." });
        }
    }
}
