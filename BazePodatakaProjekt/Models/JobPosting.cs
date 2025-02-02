using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;
using static System.Net.Mime.MediaTypeNames;

namespace BazePodatakaProjekt.Models
{
    public class JobPosting
    {
        public int Id { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public string Company { get; set; }
        [Required]
        public string Location { get; set; }
        public DateTime PostedDate { get; set; } = DateTime.UtcNow;
        public bool IsApproved { get; set; }
        [Required]
        public string UserId { get; set; }
        [ForeignKey(nameof(UserId))]
        public IdentityUser User { get; set; }
        public int LikeCount { get; set; }
        public ICollection<Like> Likes { get; set; } = new List<Like>();
        public int CategoryId { get; set; }

        [ForeignKey(nameof(CategoryId))]
        public Category Category { get; set; }

        public decimal Price { get; set; }

        public string Condition { get; set; }

        public ICollection<Image> Images { get; set; } = new List<Image>();
        public ICollection<Review> Reviews { get; set; } = new List<Review>();
    }
}
