using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace P03_FootballBetting.Data.Models
{
    public class Color
    {
        [Required]
        public int ColorId { get; set; }

        [Required]
        [MaxLength(40)]
        public string Name { get; set; }

        [InverseProperty("PrimaryKitColor")]
        public ICollection<Team> PrimaryKitTeams { get; set; } = new List<Team>();

        [InverseProperty("SecondaryKitColor")]
        public ICollection<Team> SecondaryKitTeams { get; set; } = new List<Team>();
    }
}
