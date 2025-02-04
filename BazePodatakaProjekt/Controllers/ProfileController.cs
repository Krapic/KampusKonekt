using BazePodatakaProjekt.Constants;
using BazePodatakaProjekt.Data;
using BazePodatakaProjekt.Models;
using BazePodatakaProjekt.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace BazePodatakaProjekt.Controllers
{
    [Authorize]
    public class ProfileController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        public ProfileController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        // Akcija za prikaz profila korisnika
        public async Task<IActionResult> Index()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return NotFound();
            }
            // Dohvati UserProfile iz baze
            var userProfile = await _context.UserProfiles
                .FirstOrDefaultAsync(up => up.UserId == currentUser.Id);
            var userProfileViewModel = new UserProfileViewModel
            {
                User = currentUser,
                JobPostings = await _context.JobPostings
                    .Where(jp => jp.UserId == currentUser.Id)
                    .ToListAsync(),
                            Followers = await _context.UserFollows
                    .Where(uf => uf.FollowedId == currentUser.Id)
                    .Select(uf => uf.Follower)
                    .ToListAsync(),
                            Following = await _context.UserFollows
                    .Where(uf => uf.FollowerId == currentUser.Id)
                    .Select(uf => uf.Followed)
                    .ToListAsync(),
                EditProfile = new UserProfileEditViewModel
                {
                    FirstName = userProfile?.FirstName ?? currentUser.UserName,
                    LastName = userProfile?.LastName ?? "",
                    Email = currentUser.Email,
                    PhoneNumber = currentUser.PhoneNumber,
                    Faculty = userProfile?.Faculty ?? "",
                    Bio = userProfile?.Bio ?? "",
                    ProfilePicture = userProfile?.ProfilePicture ?? ""
                }
            };

            if (User.IsInRole(Roles.Admin))
            {
                ViewBag.AllJobPostings = await _context.JobPostings
                     .Include(jp => jp.User)
                     .Include(jp => jp.Likes)
                     .Include(jp => jp.Category)
                     .Include(jp => jp.Images)
                     .Include(jp => jp.Reviews)
                     .ToListAsync();
            }

            return View(userProfileViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditProfile([FromBody] UserProfileEditViewModel model)
        {
            Console.WriteLine("EditProfile metoda pozvana");

            // Ispis svih podataka
            Console.WriteLine($"FirstName: {model.FirstName}");
            Console.WriteLine($"LastName: {model.LastName}");
            Console.WriteLine($"Email: {model.Email}");
            Console.WriteLine($"PhoneNumber: {model.PhoneNumber}");
            Console.WriteLine($"Faculty: {model.Faculty}");
            Console.WriteLine($"Bio: {model.Bio}");
            Console.WriteLine($"ProfilePicture: {model.ProfilePicture}");

            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                Console.WriteLine("ModelState greške:");
                foreach (var error in errors)
                {
                    Console.WriteLine(error);
                }

                return Json(new { success = false, errors });
            }

            // Dohvati trenutnog korisnika
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Json(new { success = false, message = "Korisnik nije pronađen." });
            }

            // Ažurirajte osnovne podatke korisnika (IdentityUser)
            user.Email = model.Email;
            user.PhoneNumber = model.PhoneNumber;

            // Ažurirajte korisnika u bazi
            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => e.Description).ToList();
                return Json(new { success = false, errors });
            }

            // Ažurirajte UserProfile podatke
            var userProfile = await _context.UserProfiles.FirstOrDefaultAsync(up => up.UserId == user.Id);
            if (userProfile == null)
            {
                // Ako UserProfile ne postoji, kreirajte ga
                userProfile = new UserProfile
                {
                    UserId = user.Id,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Faculty = model.Faculty,
                    Bio = model.Bio,
                    ProfilePicture = model.ProfilePicture
                };
                _context.UserProfiles.Add(userProfile);
            }
            else
            {
                // Ako UserProfile postoji, ažurirajte ga
                userProfile.FirstName = model.FirstName;
                userProfile.LastName = model.LastName;
                userProfile.Faculty = model.Faculty;
                userProfile.Bio = model.Bio;
                userProfile.ProfilePicture = model.ProfilePicture;
            }

            // Spremite promjene u bazu podataka
            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "Profil uspješno ažuriran." });
        }
    }
}