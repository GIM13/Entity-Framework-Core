using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace P03_FootballBetting.Data.Models
{
    public class User
    {  
        [Required]
        public int UserId { get; set; }

        [Required]
        [MaxLength(40)]
        public string Username { get; set; }

        [Required]
        [PasswordPropertyText]
        public string Password { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [MaxLength(40)]
        public string Name { get; set; }

        [Required]
        public decimal Balance { get; set; }

        public ICollection<Bet> Bets { get; set; } = new List<Bet>();
    }
}
