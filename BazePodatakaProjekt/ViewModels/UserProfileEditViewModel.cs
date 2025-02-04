using System.ComponentModel.DataAnnotations;
using BazePodatakaProjekt.Models;
using Microsoft.AspNetCore.Identity;

namespace BazePodatakaProjekt.ViewModels
{
    public class UserProfileEditViewModel
    {
        [Required(ErrorMessage = "Ime je obavezno.")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Prezime je obavezno.")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Email je obavezan.")]
        [EmailAddress(ErrorMessage = "Neispravan format emaila.")]
        public string Email { get; set; }

        [Phone(ErrorMessage = "Neispravan format broja telefona.")]
        public string PhoneNumber { get; set; }

        public string Faculty { get; set; }
        public string Bio { get; set; }
        public string ProfilePicture { get; set; }
    }
}
