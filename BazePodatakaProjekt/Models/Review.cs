using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace BazePodatakaProjekt.Models
{
    public class Review
    {
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; } // Korisnik koji ostavlja recenziju

        [ForeignKey(nameof(UserId))]
        public IdentityUser User { get; set; }

        [Required]
        public int JobPostingId { get; set; } // Oglas o kojem je recenzija

        [ForeignKey(nameof(JobPostingId))]
        public JobPosting JobPosting { get; set; }

        [Range(1, 5)]
        public int Rating { get; set; } // Ocjena (1-5)

        public string Comment { get; set; } // Komentar

        public DateTime ReviewDate { get; set; } = DateTime.UtcNow; // Datum recenzije
    }
}