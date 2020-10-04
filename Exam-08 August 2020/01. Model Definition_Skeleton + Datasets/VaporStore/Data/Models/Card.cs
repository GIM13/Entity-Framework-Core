using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using VaporStore.Data.Models.Enums;

namespace VaporStore.Data.Models
{
    public class Card
    {
        public int Id { get; set; }

        [Required/*,RegularExpression()*/]
        public string Number { get; set; }
        //•	Number – text, which consists of 4 pairs of 4 digits, separated by spaces(ex. “1234 5678 9012 3456”) (required)

        [StringLength(3, MinimumLength = 3), Required]
        public string Cvc { get; set; }

        [Required]
        public CardType Type { get; set; }

        [Required]
        public int UserId { get; set; }
        [Required]
        public User User { get; set; }

        public ICollection<Purchase> Purchases { get; set; } = new HashSet<Purchase>();
    }
}
