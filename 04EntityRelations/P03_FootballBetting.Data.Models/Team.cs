using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace P03_FootballBetting.Data.Models
{
    public class Team
    {
        [Required]
        public int TeamId { get; set; }

        [Required]
        [MaxLength(40)]
        public string Name { get; set; }

        [Required]
        [MaxLength(400)]
        public string LogoUrl { get; set; }

        [Required]
        [MaxLength(3)]
        public string Initials { get; set; }

        [Required]
        public decimal Budget { get; set; }

        [ForeignKey("Color")]
        public int PrimaryKitColorId { get; set; }
        public Color PrimaryKitColor { get; set; }

        [ForeignKey("Color")]
        public int SecondaryKitColorId { get; set; }
        public Color SecondaryKitColor { get; set; }

        [Required]
        public int TownId { get; set; }
        public Town Town { get; set; }

        [InverseProperty("AwayTeam")]
        public ICollection<Game> AwayGames { get; set; } = new List<Game>();

        [InverseProperty("HomeTeam")]
        public ICollection<Game> HomeGames { get; set; } = new List<Game>();

        public ICollection<Player> Players { get; set; } = new List<Player>();
    }
}
