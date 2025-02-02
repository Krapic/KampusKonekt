using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BazePodatakaProjekt.Models
{
    public class Image
    {
        public int Id { get; set; }

        [Required]
        public string Url { get; set; } // URL slike

        public int JobPostingId { get; set; } // Oglas kojem pripada slika

        [ForeignKey(nameof(JobPostingId))]
        public JobPosting JobPosting { get; set; }
    }
}