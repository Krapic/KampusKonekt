using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BazePodatakaProjekt.Models
{
    public class Category
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } // Naziv kategorije

        public string Description { get; set; } // Opis kategorije

        public ICollection<JobPosting> JobPostings { get; set; } = new List<JobPosting>(); // Lista oglasa u ovoj kategoriji
    }
}