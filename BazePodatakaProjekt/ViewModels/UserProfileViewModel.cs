using System.ComponentModel.DataAnnotations;
using BazePodatakaProjekt.Models;
using Microsoft.AspNetCore.Identity;

namespace BazePodatakaProjekt.ViewModels
{
    public class UserProfileViewModel
    {
        public IdentityUser User { get; set; }
        public List<JobPosting> JobPostings { get; set; }
        public List<IdentityUser> Followers { get; set; }
        public List<IdentityUser> Following { get; set; }
        public UserProfileEditViewModel EditProfile { get; set; }
        public IEnumerable<JobPosting> AllJobPostings { get; set; }
    }
}