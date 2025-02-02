using System.ComponentModel.DataAnnotations;

namespace BazePodatakaProjekt.ViewModels
{
    public class JobPostingViewModel
    {
        [Required]
        public string Title { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public string Company { get; set; }

        [Required]
        public string Location { get; set; }

        [Required]
        public int CategoryId { get; set; } // Nova kategorija oglasa

        [Required]
        [Range(0.01, 1000000, ErrorMessage = "Cijena mora biti veća od 0.")]
        public decimal Price { get; set; } // Cijena oglasa

        [Required]
        public string Condition { get; set; } // Stanje predmeta

        public IFormFile? Image { get; set; } // Slika oglasa
    }
}
