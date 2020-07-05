using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace P03_FootballBetting.Data.Models
{
    public class Country
    {
        [Required]
        public int CountryId { get; set; }

        [Required]
        [MaxLength(40)]
        public string Name { get; set; }

        public ICollection<Town> Towns { get; set; } = new List<Town>();
    }
}
