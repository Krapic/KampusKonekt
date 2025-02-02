using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BazePodatakaProjekt.Models
{
    public class UserProfile
    {
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; } 

        [ForeignKey(nameof(UserId))]
        public IdentityUser User { get; set; }

        [Required]
        public string FirstName { get; set; } 

        [Required]
        public string LastName { get; set; } 

        public string Faculty { get; set; } 

        public string ProfilePicture { get; set; } 

        public string Bio { get; set; }
    }

}