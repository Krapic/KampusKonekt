using BazePodatakaProjekt.Models;

namespace BazePodatakaProjekt.ViewModels
{
    public class JobPostingWithFollowStatusViewModel
    {
        public JobPosting JobPosting { get; set; }
        public bool IsFollowing { get; set; } // Dodajemo informaciju o tome je li korisnik prati autora oglasa
    }
}