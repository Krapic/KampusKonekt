using BazePodatakaProjekt.Models;

namespace BazePodatakaProjekt.ViewModels
{
    public class JobPostingWithFollowStatusViewModel
    {
        public JobPosting JobPosting { get; set; }
        public bool IsFollowing { get; set; }
        public IEnumerable<Review> Reviews { get; set; }
    }
}