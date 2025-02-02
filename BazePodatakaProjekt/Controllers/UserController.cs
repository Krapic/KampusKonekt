using BazePodatakaProjekt.Data;
using BazePodatakaProjekt.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace BazePodatakaProjekt.Controllers
{
    [Authorize]
    public class UserController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public UserController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpPost]
        public async Task<IActionResult> FollowUser(string userId)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return Unauthorized();
            }

            var userToFollow = await _userManager.FindByIdAsync(userId);
            if (userToFollow == null)
            {
                return NotFound("User not found.");
            }
            var existingFollow = await _context.UserFollows
                .FirstOrDefaultAsync(uf => uf.FollowerId == currentUser.Id && uf.FollowedId == userId);

            if (existingFollow != null)
            {
                return BadRequest("You are already following this user.");
            }
            var userFollow = new UserFollow
            {
                FollowerId = currentUser.Id,
                FollowedId = userId
            };

            _context.UserFollows.Add(userFollow);
            await _context.SaveChangesAsync();

            return Ok("You are now following this user.");
        }

        [HttpPost]
        public async Task<IActionResult> UnfollowUser(string userId)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return Unauthorized();
            }

            var existingFollow = await _context.UserFollows
                .FirstOrDefaultAsync(uf => uf.FollowerId == currentUser.Id && uf.FollowedId == userId);

            if (existingFollow == null)
            {
                return BadRequest("You are not following this user.");
            }
            _context.UserFollows.Remove(existingFollow);
            await _context.SaveChangesAsync();

            return Ok("You have unfollowed this user.");
        }
    }
}