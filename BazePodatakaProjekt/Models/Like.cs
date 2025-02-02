using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace BazePodatakaProjekt.Models
{
    public class Like
    {
        public int Id { get; set; }
        public string UserId { get; set; } // Korisnik koji je lajkao
        public int JobPostingId { get; set; } // Objava koja je lajkana

        [ForeignKey(nameof(UserId))]
        public IdentityUser User { get; set; }

        [ForeignKey(nameof(JobPostingId))]
        public JobPosting JobPosting { get; set; }
    }
}