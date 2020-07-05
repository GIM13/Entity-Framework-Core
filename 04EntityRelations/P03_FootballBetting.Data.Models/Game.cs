using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace P03_FootballBetting.Data.Models
{
    public class Game
    {
        [Required]
        public int GameId { get; set; }

        [Required]
        [ForeignKey("Team")]
        public int HomeTeamId { get; set; }
        public Team HomeTeam { get; set; }

        [Required]
        [ForeignKey("Team")]
        public int AwayTeamId { get; set; }
        public Team AwayTeam { get; set; }


        [Required]
        public int HomeTeamGoals { get; set; }

        [Required]
        public int AwayTeamGoals { get; set; }

        [Required]
        public DateTime DateTime  { get; set; }

        [Required]
        public double HomeTeamBetRate { get; set; }

        [Required]
        public double AwayTeamBetRate { get; set; }

        [Required]
        public double DrawBetRate { get; set; }

        [Required]
        [MaxLength(7)]
        public string Result { get; set; }

        public ICollection<Bet> Bets { get; set; } = new List<Bet>();

        public ICollection<PlayerStatistic> PlayerStatistics { get; set; } = new List<PlayerStatistic>();
    }                                       
}
