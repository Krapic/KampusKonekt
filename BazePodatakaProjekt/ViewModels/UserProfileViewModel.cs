using System.ComponentModel.DataAnnotations;
using BazePodatakaProjekt.Models;
using Microsoft.AspNetCore.Identity;

namespace BazePodatakaProjekt.ViewModels
{
    public class UserProfileViewModel
    {
        public IdentityUser User { get; set; } // Osnovni podaci o korisniku
        public List<JobPosting> JobPostings { get; set; } // Objavljeni oglasi
        public List<IdentityUser> Followers { get; set; } // Korisnici koji prate ovog korisnika
        public List<IdentityUser> Following { get; set; } // Korisnici koje ovaj korisnik prati
        public UserProfileEditViewModel EditProfile { get; set; } // Podaci za uređivanje profila
    }
}