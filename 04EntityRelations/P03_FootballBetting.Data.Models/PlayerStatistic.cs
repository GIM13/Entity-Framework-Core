using System.ComponentModel.DataAnnotations;

namespace P03_FootballBetting.Data.Models
{
    public class PlayerStatistic
    {
        [Required]
        public int GameId { get; set; }
        public Game Game { get; set; }

        [Required]
        public int PlayerId { get; set; }
        public Player Player { get; set; }

        [Required]
        public int ScoredGoals { get; set; }

        [Required]
        public int Assists { get; set; }

        [Required]
        public int MinutesPlayed { get; set; }
    }
}
