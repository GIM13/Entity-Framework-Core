using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace P03_FootballBetting.Data.Models
{
    public class Position
    {
        [Required]
        public int PositionId { get; set; }

        [Required]
        [MaxLength(40)]
        public string Name { get; set; }

        public ICollection<Player> Players { get; set; } = new List<Player>();
    }
}
