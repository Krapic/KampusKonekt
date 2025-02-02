using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace BazePodatakaProjekt.Models
{
    public class UserFollow
    {
        public string FollowerId { get; set; } // ID korisnika koji prati
        public string FollowedId { get; set; } // ID korisnika koji se prati

        [ForeignKey("FollowerId")]
        public IdentityUser Follower { get; set; }

        [ForeignKey("FollowedId")]
        public IdentityUser Followed { get; set; }
    }
}